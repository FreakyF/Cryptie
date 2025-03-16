# API specification

## Data Format

All data transmitted between components must be in JSON format.  
**Encoding:** Unicode

## Versioning Strategy

The API version will be specified in the URL, e.g., `/api/v1/...`.

When introducing backward-incompatible changes, the version number must be incremented.

## HTTP Methods

* GET: Retrieve data
* POST: Create new resources
* PATCH: Update existing resources
* DELETE: Delete resources

# Endpoints

### Account Management

* POST `/api/v1/signup`
* POST `/api/v1/auth`
* GET `/api/v1/account/{accountID}`
* DELETE `/api/v1/account/{accountID}`
* PATCH `/api/v1/account/{accountID}`

### Friends Management

* GET `/api/v1/friends`
* POST `/api/v1/friends/{accountID}`
* DELETE `/api/v1/friends/{accountID}`

### Groups Management

* GET `/api/v1/groups`
* GET `/api/v1/groups/{groupID}`
* GET `/api/v1/groups/{groupID}/members`
* POST `/api/v1/groups`
* POST `/api/v1/groups/{groupID}/members/{accountID}`
* DELETE `/api/v1/groups/{groupID}/members/{accountID}`
* PATCH `/api/v1/groups/{groupID}`
* DELETE `/api/v1/groups/{groupID}`

### Communication

* GET `/api/v1/ws`
* GET `/api/v1/chat/{chatID}`

### Keys Management

* GET `/api/v1/keys/public`
* GET `/api/v1/keys/private`
* POST `/api/v1/keys/private`
* DELETE `/api/v1/keys/private`
