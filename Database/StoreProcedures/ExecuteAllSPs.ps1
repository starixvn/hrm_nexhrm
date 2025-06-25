# PowerShell script to execute all stored procedures
$server = "10.18.200.68,1435"
$database = "NexVisionHRM"
$folderPath = "D:\Project\HRM\nexhrm\Database\StoreProcedures"
$username = "vnNovaStar"
$password = "vnNovaStar123!@#"

Get-ChildItem -Path $folderPath -Filter "*.sql" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'USE \[.*?\]', "USE [$database]"
    $tempFile = [System.IO.Path]::GetTempFileName()
    $content | Set-Content $tempFile
    sqlcmd -S $server -d $database -U $username -P $password -i $tempFile
    Remove-Item $tempFile
    Write-Host "Executed: $($_.Name)" -ForegroundColor Green
}
