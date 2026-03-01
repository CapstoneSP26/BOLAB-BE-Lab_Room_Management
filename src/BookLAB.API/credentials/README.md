# Google Calendar API Credentials

## Setup Instructions

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the Google Calendar API
4. Create credentials (Service Account)
5. Download the JSON key file
6. Rename it to `google-calendar-credentials.json`
7. Place it in this folder

## Required Scopes
- `https://www.googleapis.com/auth/calendar`

## Service Account Setup
1. In Google Cloud Console, go to **IAM & Admin** > **Service Accounts**
2. Create a new service account
3. Grant necessary permissions
4. Create a JSON key
5. Download and save it here as `google-calendar-credentials.json`

## Sharing Calendar with Service Account
After creating the service account:
1. Copy the service account email (e.g., `booklab@your-project.iam.gserviceaccount.com`)
2. Open Google Calendar
3. Go to calendar settings
4. Share your calendar with the service account email
5. Give it "Make changes to events" permission

## File Structure
```json
{
  "type": "service_account",
  "project_id": "your-project-id",
  "private_key_id": "your-key-id",
  "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
  "client_email": "booklab@your-project.iam.gserviceaccount.com",
  "client_id": "your-client-id",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/..."
}
```

## Security Note
⚠️ **Never commit this file to version control!**
- Add `credentials/` to your `.gitignore`
- Keep this file secure and private
