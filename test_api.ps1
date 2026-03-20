$headers = @{
    'Content-Type' = 'application/json'
}
try {
    $response = Invoke-WebRequest -Uri 'http://localhost:5047/api/dashboard/stats' -Headers $headers -UseBasicParsing -ErrorAction Stop
    Write-Host "Status: $($response.StatusCode)"
    Write-Host "Content: $($response.Content)"
} catch {
    Write-Host "Error: $($_.Exception.Response.StatusCode)"
    Write-Host "Message: $($_.Exception.Message)"
}