[CmdletBinding()]
param(
    [ValidateSet('Lfs', 'Text')]
    [string]$Kind = 'Lfs',

    [ValidateRange(1, 4096)]
    [int]$BatchMiB = 128,

    [switch]$Execute
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location -LiteralPath $repositoryRoot
$safeDirectoryArgument = "safe.directory=$($repositoryRoot -replace '\\', '/')"
$lfsExtensions = @(
    '.png', '.jpg', '.jpeg', '.tga', '.tif', '.tiff', '.exr', '.hdr', '.psd',
    '.aup3', '.wav', '.mp3', '.ogg', '.mp4',
    '.fbx', '.obj', '.blend', '.hda', '.otl', '.zip',
    '.ttf', '.otf', '.pdf', '.dll'
)
$allowedRoots = @('Assets', 'Packages', 'ProjectSettings')
$forbiddenRoots = @('Library', 'Temp', 'Obj', 'Build', 'Builds', 'Logs', 'UserSettings', 'MemoryCaptures', '.git')
$batchBytesLimit = [int64]$BatchMiB * 1MB

function Invoke-Git {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments)

    & git -c $safeDirectoryArgument @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Git command failed: git $($Arguments -join ' ')"
    }
}

function Get-RepositoryPath {
    param([System.IO.FileInfo]$File)

    return ($File.FullName.Substring($repositoryRoot.Length + 1) -replace '\\', '/')
}

function Test-AllowedPath {
    param([string]$Path)

    $root = ($Path -split '[\\/]', 2)[0]
    return $allowedRoots -contains $root -and -not ($forbiddenRoots -contains $root)
}

if ((Invoke-Git rev-parse --is-inside-work-tree) -ne 'true') {
    throw "The script must run inside a Git worktree."
}

if ((Invoke-Git branch --show-current) -ne 'main') {
    throw "Refusing to publish from any branch other than main."
}

Invoke-Git remote get-url origin | Out-Null

& git -c $safeDirectoryArgument diff --cached --quiet
if ($LASTEXITCODE -ne 0) {
    throw "The index already contains staged changes. Commit, unstage, or stash them before running this publisher."
}

& git -c $safeDirectoryArgument diff --quiet
if ($LASTEXITCODE -ne 0) {
    throw "Tracked changes are present. This recovery publisher only handles the rebuilt, untracked project baseline."
}

& git -c $safeDirectoryArgument rev-list --quiet origin/main..main
if ($LASTEXITCODE -eq 0) {
    $aheadCommits = @(git -c $safeDirectoryArgument rev-list origin/main..main)
    if ($aheadCommits.Count -gt 0) {
        throw "main is ahead of origin/main. Push or resolve the existing commit before creating another batch."
    }
}

$candidatePaths = @(
    git -c $safeDirectoryArgument ls-files --others --exclude-standard |
        Where-Object { $_ -and (Test-AllowedPath $_) }
)

$candidates = foreach ($candidatePath in $candidatePaths) {
    $file = Get-Item -LiteralPath (Join-Path $repositoryRoot $candidatePath) -ErrorAction SilentlyContinue
    if (-not $file -or $file.PSIsContainer) {
        continue
    }

    $isLfs = $lfsExtensions -contains $file.Extension.ToLowerInvariant()
    if (($Kind -eq 'Lfs' -and $isLfs) -or ($Kind -eq 'Text' -and -not $isLfs)) {
        $file
    }
}

$candidates = @($candidates | Sort-Object { Get-RepositoryPath $_ })
if ($candidates.Count -eq 0) {
    Write-Host "No $Kind candidates remain."
    exit 0
}

$batches = [System.Collections.Generic.List[object]]::new()
$currentBatch = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
[int64]$currentBatchBytes = 0

foreach ($candidate in $candidates) {
    if ($currentBatch.Count -gt 0 -and ($currentBatchBytes + $candidate.Length) -gt $batchBytesLimit) {
        $batches.Add([PSCustomObject]@{ Files = @($currentBatch); Bytes = $currentBatchBytes })
        $currentBatch = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
        $currentBatchBytes = 0
    }

    $currentBatch.Add($candidate)
    $currentBatchBytes += $candidate.Length
}

if ($currentBatch.Count -gt 0) {
    $batches.Add([PSCustomObject]@{ Files = @($currentBatch); Bytes = $currentBatchBytes })
}

Write-Host "$Kind publisher plan: $($candidates.Count) files in $($batches.Count) batch(es), capped at $BatchMiB MiB."
for ($index = 0; $index -lt $batches.Count; $index++) {
    $batch = $batches[$index]
    $samplePaths = @($batch.Files | Select-Object -First 3 | ForEach-Object { Get-RepositoryPath $_ }) -join ', '
    Write-Host ('[{0}/{1}] {2} files, {3:N2} MiB: {4}' -f ($index + 1), $batches.Count, $batch.Files.Count, ($batch.Bytes / 1MB), $samplePaths)
}

if (-not $Execute) {
    Write-Host 'Dry run only. Re-run with -Execute to commit and push these batches.'
    exit 0
}

for ($index = 0; $index -lt $batches.Count; $index++) {
    $batch = $batches[$index]
    $pathsToStage = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $batch.Files) {
        $relativePath = Get-RepositoryPath $file
        $pathsToStage.Add($relativePath)

        $metaPath = "$($file.FullName).meta"
        if (Test-Path -LiteralPath $metaPath -PathType Leaf) {
            $pathsToStage.Add((Get-RepositoryPath (Get-Item -LiteralPath $metaPath)))
        }
    }

    $uniquePaths = @($pathsToStage | Sort-Object -Unique)
    Invoke-Git add -- @uniquePaths
    & git -c $safeDirectoryArgument diff --cached --quiet
    if ($LASTEXITCODE -eq 0) {
        throw "Batch $($index + 1) did not produce staged changes."
    }

    $message = 'Add {0} assets batch {1} of {2} ({3:N2} MiB)' -f $Kind.ToUpperInvariant(), ($index + 1), $batches.Count, ($batch.Bytes / 1MB)
    Invoke-Git commit -m $message

    & git -c $safeDirectoryArgument -c lfs.concurrenttransfers=1 push origin main
    if ($LASTEXITCODE -ne 0) {
        throw "Push failed after committing batch $($index + 1). The commit remains local; resolve the push before running the publisher again."
    }
}

Write-Host "Published $($batches.Count) $Kind batch(es) successfully."
