<h1 align="center">
Quinn Estate

<img alt="Capture.PNG" src="https://github.com/QuinnPeterson/QuinnPeterson/blob/main/projects/real%20estate/Capture.PNG?raw=true" data-hpc="true" class="Box-sc-g0xbh4-0 kzRgrI">

</h1>

<h4 align="center">A Real Estate Fullstack Web App made in React and .NET</h4>

## Technologies Used
- **Frontend**:
  - React
  - Tailwind 
  - Vite

- **Backend**:
  - C#
  - ASP.NET
  - JWT and BCrypt

- **Database**:
  - Firebase Realtime Database

## Key Features
- **User Login and Registration**: Secure user login powered by Firebase and C#.
- **Real-Time Data Updates**: Utilizing Firebase Realtime Database to ensure instant updates on property listings.
- **Create, Read, Update and Delete Property Listings**: Developed the app allowing users to create, read, update and delete listings in realtime.
- **Backend with ASP.NET**: The server-side logic is implemented using C# and ASP.NET to handle various functionalities.


## Prerequisites

Before you begin, ensure you have downloaded the following requirements:

- [.Net 6.0 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed on your machine.
- [NodeJs](https://nodejs.org/en/download/) installed on your machine.

## Installation

To clone and run this application, you will need [Git](https://git-scm.com) installed on your computer. From your command line:

```bash
# Clone this repository
$ git clone https://github.com/quinnpeterson/real-estate-app

# Install frontend dependencies:
$ cd client
$ npm install

# Install backend dependencies:
$ cd server
$ dotnet restore

#Firebase Setup:
Create a Firebase project: Firebase Console.
Obtain Firebase configuration details.
Update the configuration in client/src/firebaseConfig.js.
Update the configuration in server/properties/firebaseConfig.json

# Start the frontend:
$cd client
$npm start
# Start the backend:
$cd server
$dotnet run

# The application will be accessible at http://localhost:5173

```

> **Note**
> If you're using Linux Bash for Windows, [see this guide](https://www.howtogeek.com/261575/how-to-run-graphical-linux-desktop-applications-from-windows-10s-bash-shell/) or use `node` from the command prompt.

## Sign In, Auth and Profile Pic Update

https://github.com/QuinnPeterson/Real-Estate-Project/assets/63170635/9d3579d2-2cb6-49e7-8014-61487823172e


## Search

https://github.com/QuinnPeterson/Real-Estate-Project/assets/63170635/a64e614c-3367-462f-b8d8-dd0ef40a7979


## Create Listing

https://github.com/QuinnPeterson/Real-Estate-Project/assets/63170635/8b24da08-fa1b-45d0-978d-7e069ae3c11a


## Update and Delete Listings

https://github.com/QuinnPeterson/Real-Estate-Project/assets/63170635/00fcb9f1-e2f4-4ff9-89b8-d6435023e840



> GitHub [@QuinnPeterson](https://github.com/QuinnPeterson) &nbsp;&middot;&nbsp;
> LinkedIn [@QuinnPeterson](https://www.linkedin.com/in/quinn-peterson-software-engineer/)


#Project Structure
The .NET server  is organized into the following folders:

#_QUINN
Contains the requests and responses to the server.

#Controllers
Define the end points/routes for the API, controller action methods are the entry points into the API for client applications via HTTP requests.

#Models
Represent request and response models for controller methods, request models define the parameters for incoming requests, and response models define the returned data.

#Services
Contains business logic, validation, and database access code.

#Properties
Contains the launchSettings.json file that sets the environment `(ASPNETCORE_ENVIRONMENT)` to Development by default when running the API on your local machine.
---

