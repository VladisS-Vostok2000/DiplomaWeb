# DiplomaWeb
Diploma application for Dubna university.

# Abstract
This api project can handle notes with its versions as branch. You may push and download notes on remote server by its API.

# Installing
You may test this application by docker.

Please use this [release](https://github.com/VladisS-Vostok2000/DiplomaWeb/releases/tag/Docker-compose) to download
`docker-compose.yml` file.

Verify you dont have already "db" container.

Verify your `80` and `55432` ports are free.

Go to your command console in folder with `docker-compose.yml` and run following command:

```
docker-compose -p "diplomaweb" up
```

It will create 2 containers: `db` for database and `DiplomaWebContainer` for application.

# Usage

## Users api

To test connection for user controller use:
```
GET http://localhost:80/api/Users/Ping
```
You must get `Pong.` message

To register user use:
```
POST http://localhost:80/api/Users/Register
{
 "Login": "yourLogin",
 "Password": "yourPassword"
}
```
You will get bearer token as response.

To login (getting bearer token) use:
```
GET http://localhost:80/api/Users/Login
{
 "Login": "yourLogin",
 "Password": "yourPassword"
}
```

## Notes api

To test connection for notes controller use:
```
GET http://localhost:80/api/Notes/Ping
```
You must get `Pong.` message.

For the following api's please use bearer token.

To post the note the minimum as you need is:
```
POST http://localhost:80/api/Notes/Note
{
 "Name": "yourNoteName",
 "Text": "yourNoteText"
}
```
You will get posted note as response like following:
```
{
    "id": 0,
    "userName": "yourLogin",
    "name": "yourNoteName",
    "branchName": "yourNoteName",
    "version": 0,
    "text": "yourNoteText",
    "creatingDate": "2023-05-22T16:26:57.2556158+03:00"
}
```
Note branch name created automatically here.

As maximum you may use following json:
```
{
  "Name": "yourNoteName",
  "BranchName": "yourNoteBranchName",
  "Text": "yourNoteText",
  "CreatingDate": "dateOfCreating"
}
```
Note that you must use `BranchName` to post your note in existing branch.

To get note use:
```
GET http://localhost:80/api/Notes/Note?branchName=yourNoteBranchName&version=0
```
Where `yourNoteBranchName` and `version` are parameters.

To pull all user notes please use:
```
GET http://localhost:80/api/Notes/Notes
```

To remove branch use:
```
DELETE http://localhost:80/api/Notes/Note?branchName=yourNoteBranchName
```
where `yourNoteBranchName` is parameter.


