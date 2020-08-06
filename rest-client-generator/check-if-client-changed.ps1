$diff = git diff --stat
if ($null -eq $diff) {
    Write-Host "Found 0 files in diff."
}
else {
    $clientFiles = $diff |
        % { $_.Trim().Split(" ")[0] } | # Remove leading whitespace and trailing count of +/- lines
        Where-Object {
            ($_.StartsWith("web2/src/api-client/")) -and `
            ($_ -ne "web2/src/api-client/.openapi-generator/VERSION") # This shows up as 'unset' when run in pipeline for some reason
        }

    if ($clientFiles.count -gt 0) {
        Write-Host "Found $($diff.count) files in diff, including $($clientFiles.count) API client files." -ForegroundColor Red

        foreach ($file in $clientFiles) {
            Write-Host $file
            $relativePath = "../$file"
            Get-Content $relativePath
        }

        throw "API client has not been regenerated since last API contract changes."
    }
    else {
        Write-Host "Found $($diff.count) files in diff, but no API client files."
    }
}