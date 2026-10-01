$baseUrl = "http://localhost:5111"

$notificationProject = `
    "C:\TexasMediaDart\Notification\src\TexasMediaDart.Notification.Api"

$recipientEmail = "antolearn@gmail.com"

$body = @{
    recipientEmail  = $recipientEmail
    verificationUrl = "http://localhost:5000/#/verify-email?token=NOTIFICATION-SMOKE-TEST"
} | ConvertTo-Json


# ============================================================
# Test 1 - No API key
# ============================================================

Write-Host ""
Write-Host "============================================"
Write-Host " Test 1 - Request WITHOUT API key"
Write-Host "============================================"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -ContentType "application/json" `
        -Body $body `
        -UseBasicParsing

    Write-Host "[FAIL] Request was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode = $null

    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
    }

    if ($statusCode -eq 401) {
        Write-Host "[PASS] Request rejected without API key."
        Write-Host "Status: 401"
    }
    else {
        Write-Host "[FAIL] Unexpected response."
        Write-Host "Status:" $statusCode
        Write-Host "Error :" $_.Exception.Message
    }
}


# ============================================================
# Test 2 - Invalid API key
# ============================================================

Write-Host ""
Write-Host "============================================"
Write-Host " Test 2 - Request with INVALID API key"
Write-Host "============================================"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -Headers @{
            "X-API-Key" = "INVALID-API-KEY"
        } `
        -ContentType "application/json" `
        -Body $body `
        -UseBasicParsing

    Write-Host "[FAIL] Invalid API key was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode = $null

    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
    }

    if ($statusCode -eq 401) {
        Write-Host "[PASS] Invalid API key rejected."
        Write-Host "Status: 401"
    }
    else {
        Write-Host "[FAIL] Unexpected response."
        Write-Host "Status:" $statusCode
        Write-Host "Error :" $_.Exception.Message
    }
}


# ============================================================
# Load real API key from User Secrets
# ============================================================

Write-Host ""
Write-Host "============================================"
Write-Host " Loading service API key"
Write-Host "============================================"

$secrets = `
    dotnet user-secrets list `
        --project $notificationProject

$keyLine = $secrets |
    Where-Object {
        $_ -like "ServiceAuthentication:ApiKey =*"
    } |
    Select-Object -First 1

if (-not $keyLine) {
    Write-Host "[FAIL] ServiceAuthentication:ApiKey not found."
    exit 1
}

$apiKey = $keyLine.Substring(
    $keyLine.IndexOf("=") + 1
).Trim()

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "[FAIL] Service API key is empty."
    exit 1
}

Write-Host "[OK] Service API key loaded."


# ============================================================
# Test 3 - Valid API key
# ============================================================

Write-Host ""
Write-Host "============================================"
Write-Host " Test 3 - Request with VALID API key"
Write-Host "============================================"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -Headers @{
            "X-API-Key" = $apiKey
        } `
        -ContentType "application/json" `
        -Body $body `
        -UseBasicParsing

    if ($response.StatusCode -eq 202) {
    Write-Host "[PASS] Valid API key accepted."
    Write-Host "Status: 202 Accepted"
    Write-Host "Body  :" $response.Content
    Write-Host ""
    Write-Host "Check inbox:"
    Write-Host $recipientEmail
}
else {
    Write-Host "[FAIL] Unexpected success status."
    Write-Host "Expected: 202"
    Write-Host "Actual  :" $response.StatusCode
    Write-Host "Body    :" $response.Content
}
}
catch {
    $statusCode = $null

    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
    }

    Write-Host "[FAIL] Valid API key request failed."
    Write-Host "Status:" $statusCode
    Write-Host "Error :" $_.Exception.Message
}


# ============================================================
# Summary
# ============================================================

Write-Host ""
Write-Host "============================================"
Write-Host " Notification Security Test Complete"
Write-Host "============================================"