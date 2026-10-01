# ============================================================
# TexasMediaDart Notification API - Local Smoke Test
# Compatible with Windows PowerShell 5.1
# ============================================================

$baseUrl = "http://localhost:5111"

$notificationProject = `
    "C:\TexasMediaDart\Notification\src\TexasMediaDart.Notification.Api"

$recipientEmail = "antolearn@gmail.com"


# ============================================================
# Helper - Get HTTP status code from exception
# ============================================================

function Get-HttpStatusCode {
    param(
        $Exception
    )

    if ($Exception.Response) {
        return [int]$Exception.Response.StatusCode
    }

    return $null
}


# ============================================================
# Load Service API Key
# ============================================================

Write-Host ""
Write-Host "============================================================"
Write-Host " TexasMediaDart Notification API Smoke Test"
Write-Host "============================================================"

Write-Host ""
Write-Host "Loading ServiceAuthentication:ApiKey..."

$secrets = `
    dotnet user-secrets list `
        --project $notificationProject

$keyLine = $secrets |
    Where-Object {
        $_ -like "ServiceAuthentication:ApiKey =*"
    } |
    Select-Object -First 1

if (-not $keyLine) {
    Write-Host "[FAIL] ServiceAuthentication:ApiKey was not found."
    exit 1
}

$apiKey = $keyLine.Substring(
    $keyLine.IndexOf("=") + 1
).Trim()

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "[FAIL] ServiceAuthentication:ApiKey is empty."
    exit 1
}

Write-Host "[OK] Service API key loaded."


# ============================================================
# Health Check
# ============================================================

Write-Host ""
Write-Host "============================================================"
Write-Host " Health Check"
Write-Host "============================================================"

try {
    $health = Invoke-RestMethod `
        -Method Get `
        -Uri "$baseUrl/health/api"

    if ($health.status -eq "Healthy") {
        Write-Host "[PASS] Notification API is healthy."
        Write-Host "Service:" $health.service
    }
    else {
        Write-Host "[FAIL] Notification API is not healthy."
        exit 1
    }
}
catch {
    Write-Host "[FAIL] Unable to reach Notification API."
    Write-Host $_.Exception.Message
    exit 1
}


# ============================================================
# EMAIL VERIFICATION
# ============================================================

Write-Host ""
Write-Host "============================================================"
Write-Host " EMAIL VERIFICATION TESTS"
Write-Host "============================================================"

$verificationBody = @{
    recipientEmail = $recipientEmail
    verificationUrl =
        "http://localhost:5000/#/verify-email?token=EMAIL-VERIFICATION-SMOKE-TEST"
} | ConvertTo-Json


# ------------------------------------------------------------
# Email Verification - Test 1
# No API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test EV-1 - WITHOUT API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -ContentType "application/json" `
        -Body $verificationBody `
        -UseBasicParsing

    Write-Host "[FAIL] Request without API key was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

    if ($statusCode -eq 401) {
        Write-Host "[PASS] Missing API key rejected."
        Write-Host "Status: 401"
    }
    else {
        Write-Host "[FAIL] Unexpected response."
        Write-Host "Status:" $statusCode
        Write-Host "Error :" $_.Exception.Message
    }
}


# ------------------------------------------------------------
# Email Verification - Test 2
# Invalid API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test EV-2 - INVALID API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -Headers @{
            "X-API-Key" = "INVALID-API-KEY"
        } `
        -ContentType "application/json" `
        -Body $verificationBody `
        -UseBasicParsing

    Write-Host "[FAIL] Invalid API key was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

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


# ------------------------------------------------------------
# Email Verification - Test 3
# Valid API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test EV-3 - VALID API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/email-verification" `
        -Headers @{
            "X-API-Key" = $apiKey
        } `
        -ContentType "application/json" `
        -Body $verificationBody `
        -UseBasicParsing

    if ($response.StatusCode -eq 202) {
        Write-Host "[PASS] Email verification accepted."
        Write-Host "Status: 202 Accepted"
        Write-Host "Body  :" $response.Content
    }
    else {
        Write-Host "[FAIL] Unexpected status."
        Write-Host "Expected: 202"
        Write-Host "Actual  :" $response.StatusCode
    }
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

    Write-Host "[FAIL] Email verification request failed."
    Write-Host "Status:" $statusCode
    Write-Host "Error :" $_.Exception.Message
}


# ============================================================
# USER INVITATION
# ============================================================

Write-Host ""
Write-Host "============================================================"
Write-Host " USER INVITATION TESTS"
Write-Host "============================================================"

$expiresUtc =
    (Get-Date).ToUniversalTime().AddHours(24)

$invitationBody = @{
    recipientEmail = $recipientEmail
    invitationUrl =
        "http://localhost:5000/#/accept-invitation?token=USER-INVITATION-SMOKE-TEST"
    expiresUtc =
        $expiresUtc.ToString("o")
} | ConvertTo-Json


# ------------------------------------------------------------
# User Invitation - Test 1
# No API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test UI-1 - WITHOUT API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/user-invitation" `
        -ContentType "application/json" `
        -Body $invitationBody `
        -UseBasicParsing

    Write-Host "[FAIL] Request without API key was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

    if ($statusCode -eq 401) {
        Write-Host "[PASS] Missing API key rejected."
        Write-Host "Status: 401"
    }
    else {
        Write-Host "[FAIL] Unexpected response."
        Write-Host "Status:" $statusCode
        Write-Host "Error :" $_.Exception.Message
    }
}


# ------------------------------------------------------------
# User Invitation - Test 2
# Invalid API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test UI-2 - INVALID API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/user-invitation" `
        -Headers @{
            "X-API-Key" = "INVALID-API-KEY"
        } `
        -ContentType "application/json" `
        -Body $invitationBody `
        -UseBasicParsing

    Write-Host "[FAIL] Invalid API key was accepted."
    Write-Host "Status:" $response.StatusCode
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

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


# ------------------------------------------------------------
# User Invitation - Test 3
# Valid API Key
# ------------------------------------------------------------

Write-Host ""
Write-Host "Test UI-3 - VALID API key"

try {
    $response = Invoke-WebRequest `
        -Method Post `
        -Uri "$baseUrl/internal/notifications/user-invitation" `
        -Headers @{
            "X-API-Key" = $apiKey
        } `
        -ContentType "application/json" `
        -Body $invitationBody `
        -UseBasicParsing

    if ($response.StatusCode -eq 202) {
        Write-Host "[PASS] User invitation accepted."
        Write-Host "Status: 202 Accepted"
        Write-Host "Body  :" $response.Content
    }
    else {
        Write-Host "[FAIL] Unexpected status."
        Write-Host "Expected: 202"
        Write-Host "Actual  :" $response.StatusCode
    }
}
catch {
    $statusCode =
        Get-HttpStatusCode $_.Exception

    Write-Host "[FAIL] User invitation request failed."
    Write-Host "Status:" $statusCode
    Write-Host "Error :" $_.Exception.Message
}


# ============================================================
# Test Summary
# ============================================================

Write-Host ""
Write-Host "============================================================"
Write-Host " Notification API Smoke Test Complete"
Write-Host "============================================================"
Write-Host ""
Write-Host "Expected results:"
Write-Host ""
Write-Host "Email Verification"
Write-Host "  EV-1 Missing API key : 401"
Write-Host "  EV-2 Invalid API key : 401"
Write-Host "  EV-3 Valid API key   : 202"
Write-Host ""
Write-Host "User Invitation"
Write-Host "  UI-1 Missing API key : 401"
Write-Host "  UI-2 Invalid API key : 401"
Write-Host "  UI-3 Valid API key   : 202"
Write-Host ""
Write-Host "Check inbox:"
Write-Host "  $recipientEmail"
Write-Host ""
Write-Host "Expected emails:"
Write-Host "  1 Email Verification email"
Write-Host "  1 User Invitation email"
Write-Host "============================================================"