# AMChat
## Please read checklist before investigating
## If something went wrong - connect to me via HR or commit message, open issue - I open for feedback
### P.S. I really wanted to implement docker compose however I had no time for it so it stayed empty as generated :(
------------
## !!!! To run integration tests - Docker Desktop must be installed at your machine
## !!!! To run solution - PostgreSQL db server must be installed at your machine.
## !!!! Also you need provide connection string in appsetting.Development.json or appsetting.json with name "DefaultConnection" or pass it via environment variables like "CONNECTIONSTRINGS__DEFAULTCONNECTION":"*ConnectionString*"
------------
## Setup
### Architecture - Onion Architecture
### Storage - EF Core (Code First) via PostgreSQL
### Application - CQRS with MediatoR
### Real-time communication - SignalR hubs
------------
## Implemented:
- #### Cross-cuttings:
1. Structured logging
2. Validation pipelines
3. Errors and exceptions handlers/filters
- #### Main functionallity
1. Quering users/chats with dynamic ordering and pagination, search by name has been implemented as well.
   User entity can be ordered with providing PropertyName == "username" or "fullname" or "birthdate"
   Chat entity can be ordered with providing PropertyName == "name" or "ownerName"
3. Creating chat with initial system message
4. Deleting chat with validating access rights
5. Updating chat with validating access rights and passing ownership if necessary
6. Join/Left chat with adding to chat messages(not confuse with connect to chat e.g. when user connect via WS to hub and open chat window)
7. Creating user/Updating user
8. Deleting user with passing ownership from owned chats to another connected user from chat. If such doesn't exists chat is removed
9. Connecting to chat with receiving previous chat messages
10. Disconnecting from chat
11. Sending messages 
12. Storage seeding with Bogus fakers
13. A lot of other actions to handle edge-cases related to tasks and domain area
- #### Mocks
1. Auth mock with taking user id from custom header and creating claims identity based on this value
2. Chat connections cache(will be better to replace with Reddis distributed cache)
- #### Testing
1. Integration testing using docker containers for database per every test class and seeding.
2. Unit testing using InMemory DbContext and Moq Mocks.
