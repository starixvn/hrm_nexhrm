# PowerShell script to execute all stored procedures
$server = "10.18.200.68,1435"  # Change this to your SQL Server instance name
$database = "NexHRM"
$folderPath = "D:\Project\HRM\nexhrm\Database\StoreProcedures"
$username = "vnNovaStar"  # Replace with your SQL Server username
$password = "vnNovaStar123!@#"  # Replace with your SQL Server password

# Get all SQL files in the folder
$files = Get-ChildItem -Path $folderPath -Filter "*.sql"

# Execute each file
foreach ($file in $files) {
    # Use sqlcmd to execute the file with SQL authentication
    sqlcmd -S $server -d $database -U $username -P $password -i $file.FullName
    Write-Host "Successfully executed $($file.Name)" -ForegroundColor Green
}
