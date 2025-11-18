# Test CreateComplaintService with AOP
Write-Host "`n=== Testing CreateComplaintService with AOP ===" -ForegroundColor Cyan
Write-Host "This will test the real complaint creation and show AOP logs`n" -ForegroundColor Yellow

$baseUrl = "http://localhost:5000"

# Step 1: Check if user exists or needs to register
Write-Host "Step 1: Do you have a user account? (y/n): " -ForegroundColor Yellow -NoNewline
$hasAccount = Read-Host

if ($hasAccount -ne "y" -and $hasAccount -ne "Y") {
    Write-Host "`nLet's register a new user first..." -ForegroundColor Green
    
    Write-Host "Enter email: " -ForegroundColor Yellow -NoNewline
    $email = Read-Host
    Write-Host "Enter password: " -ForegroundColor Yellow -NoNewline
    $password = Read-Host
    
    # Register
    Write-Host "`nRegistering user..." -ForegroundColor Gray
    try {
        $registerBody = @{
            fullName = "Test User"
            email = $email
            password = $password
            confirmPassword = $password
        } | ConvertTo-Json
        
        $registerResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -UseBasicParsing
        Write-Host "✅ Registration successful! Check your email/console for OTP code." -ForegroundColor Green
        
        Write-Host "`nEnter the OTP code from email/console: " -ForegroundColor Yellow -NoNewline
        $otp = Read-Host
        
        # Verify OTP
        Write-Host "Verifying OTP..." -ForegroundColor Gray
        $verifyBody = @{
            email = $email
            code = $otp
        } | ConvertTo-Json
        
        $verifyResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/verify-otp" -Method POST -Body $verifyBody -ContentType "application/json" -UseBasicParsing
        Write-Host "✅ Email verified!" -ForegroundColor Green
    } catch {
        Write-Host "❌ Registration error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response: $responseBody" -ForegroundColor Red
        }
        exit
    }
} else {
    Write-Host "Enter your email: " -ForegroundColor Yellow -NoNewline
    $email = Read-Host
    Write-Host "Enter your password: " -ForegroundColor Yellow -NoNewline
    $password = Read-Host
}

# Step 2: Login
Write-Host "`nStep 2: Logging in..." -ForegroundColor Green
try {
    $loginBody = @{
        email = $email
        password = $password
    } | ConvertTo-Json
    
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json" -UseBasicParsing
    $loginData = $loginResponse.Content | ConvertFrom-Json
    $token = $loginData.accessToken
    
    Write-Host "✅ Login successful!" -ForegroundColor Green
    Write-Host "   Token received: $($token.Substring(0, 20))..." -ForegroundColor Gray
} catch {
    Write-Host "❌ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
    exit
}

# Step 3: Create Complaint (This will trigger AOP!)
Write-Host "`nStep 3: Creating complaint (AOP will log performance)..." -ForegroundColor Green
try {
    $complaintBody = @{
        type = "Infrastructure"
        assignedPart = "Public Works Department"
        location = "Main Street, Building 123"
        description = "Testing AOP Performance Interceptor - pothole on the road"
    } | ConvertTo-Json
    
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    
    Write-Host "   Sending request..." -ForegroundColor Gray
    $complaintResponse = Invoke-WebRequest -Uri "$baseUrl/api/complaints" -Method POST -Headers $headers -Body $complaintBody -UseBasicParsing
    $complaintData = $complaintResponse.Content | ConvertFrom-Json
    
    Write-Host "`n✅ Complaint created successfully!" -ForegroundColor Green
    Write-Host "   Complaint Number: $($complaintData.complaintNumber)" -ForegroundColor Cyan
    Write-Host "   Complaint ID: $($complaintData.complaintId)" -ForegroundColor Cyan
    Write-Host "   Message: $($complaintData.message)" -ForegroundColor Cyan
    
    Write-Host "`n👀 NOW CHECK YOUR APPLICATION CONSOLE!" -ForegroundColor Yellow
    Write-Host "   You should see a log like:" -ForegroundColor White
    Write-Host "   [Information] Method CreateComplaintService.ExecuteAsync completed in XXms" -ForegroundColor Gray
    Write-Host "`n   This log was generated AUTOMATICALLY by AOP!" -ForegroundColor Green
    Write-Host "   You didn't write any logging code in CreateComplaintService!" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error creating complaint: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan
Write-Host "Check your application console for AOP performance logs!`n" -ForegroundColor Green

