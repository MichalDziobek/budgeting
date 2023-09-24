# Budgeting App
Simple budgeting REST API for interview task. Written in .Net 7. The application is based on CQRS pattern with MediatR and uses PostgreSql database with Entity Framework

## Setup
The only command required to start the application should be `docker compose up` in folder with solution. This will start the application on http://localhost:8080/swagger/index.html as well as PostgreSql database on default port (5432). Application run from Rider/Visual Studio uses port 7025.

Itegration tests use the same database container, so it should be started first,before running them.

The application uses Auth0 as an OAuth identity provider, and Swagger is configured to support authenticating with it. You may register new account, or user predefined admin (email is admin@example.com, password was in the email). It is necessary to create user in local db manually after first login as described below.


## Limitations/Simplifications
* Since the application relies on external identity provider, and also require some user data in database it is necessary to add newly registered users to our database.
Auth0 supports doing that using Actions: https://auth0.com/docs/customize/actions. That should be fairly simple to setup on publically available servers. This is probably the solution I would choose for production but on local environment it would require either static IP address and port forwarding, or setting up a tunneling tool like ngrok. For ngrok the public domain changes every time it is run, and static domains are a paid feature. As a workaround `POST /users` has to be called manually after first login. Data entered doesn't matter much, but it can be used to demonstrate filtering.

* Creating/deleting budget entry categories is an admin-only feature. It would make more sense to me from the "business" logic point of view if the categories were user/budget specific, but I felt it would complicate implementation a bit, while not showcasing any new skills/approaches.

* Some Delete/Update/Patch endpoints that would make sense were not implemented with the same reasoning as in the point above.