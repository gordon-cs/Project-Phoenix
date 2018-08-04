# Project-Phoenix

*Rising from the ashes of  paper RCI's , coming to a college near you.*


## Table of Contents

- [Introduction](#introduction)
    - [Clients](#clients)
    - [Team](#team)
    - [Problem Statement](#problem-statement)

- [Project-Phoenix](#project-phoenix)
    - [Table of Contents](#table-of-contents)
    - [Introduction:](#introduction)
        - [Clients:](#clients)
        - [Team:](#team)
        - [Problem Statement:](#problem-statement)
    - [Technical Documentation:](#technical-documentation)
        - [Setup:](#setup)
        - [How To:](#how-to)
            - [How do I find the project?](#how-do-i-find-the-project)
            - [How do I make changes and run the project to test?](#how-do-i-make-changes-and-run-the-project-to-test)
            - [How do I publish my changes to the live websites?](#how-do-i-publish-my-changes-to-the-live-websites)
            - [How do I change the format of the fine email?](#how-do-i-change-the-format-of-the-fine-email)
            - [How do I manually change the furniture that RCIs have?](#how-do-i-manually-change-the-furniture-that-rcis-have)
            - [How do I find the user-uploaded pictures?](#how-do-i-find-the-user-uploaded-pictures)
            - [How do I manually query the database?](#how-do-i-manually-query-the-database)
        - [Deep Dive](#deep-dive)
            - [The Database](#the-database)
            - [Authentication](#authentication)
            - [Controllers and Views](#controllers-and-views)
            - [RCI Generation](#rci-generation)
            - [Unit Tests](#unit-tests)
            - [Querying the Database](#querying-the-database)
            - [Exception Handling](#exception-handling)
        
        
    



## Introduction:

### Clients:
Director of Residence Life, Michael Curtis and Director of Housing, Marta Peralta,  
Gordon College

### Team:
Eze Anyanwu  
Stephanie Powers  
Weiqiu (Rachel) You  

### Problem Statement:
*The Problem:*

The current system for documenting the condition of rooms is redundant, ambiguous and tedious. More specifically, students use an RCI (Room Condition Inventory) to document the state of the room when they move in and move out. The RCI is a long double-sided sheet of paper with space to comment on the state of various pieces of furniture in the room.   
As a paper-based system, it has become hard to manage for the hundreds of students who live on campus.
The current RCI system is redundant because a new RCI is required from each resident every year; information from previous years is not used.   
It is ambiguous because the descriptions are subjective. What one student sees as major damage might be seen by another as minor. Accurately describing the extent of the damage also gets tricky. It is common during move-out procedures for students to protest fines for damages by saying "It was there before!".  
It is tedious because a lot of manual work has to be put in by Residence Life to make sure information gets gathered.


*Vision:*

Our goal is to create a new RCI system that would be of benefit to students, Resident Advisors and Resident Directors. What we have in mind is a web-based system with a well thought out user interface that solves the problems presented above while also simplifying the process of recording the damages in a given room.  
As further ( but important ) enhancements, the system will also provide an administrative interface for RAs and RDs. The 
interface will assist Residence Life staff in doing manual tasks ( e.g. finding who hasn’t submitted RCI’s and compiling fines ).

## Technical Documentation:

### Setup:

Throughout this readme, I'll be assuming that you have been given access to some VM that has permission to access the RCI database. I further assume that you have been given credentials with permission to log in to said Virtual Machine, publish to IIS and access the different tables that the online RCI uses.  
If you are not sure, ask whoever your contact at CTS is.

Generally speaking, the online RCI application runs as a Windows process under the user Rci.Service@gordon.edu (as of August 4th 2018). This user has read/write permissions to the Rci and RciTrain site folders as well as to the Rci and RciTrain databases. 

When you are debugging with Visual Studio, make sure that the user you open Visual Studio as also has permissions to read and write to the RCI and RCITrain databases. You'll get Login errors if that is not the case.


### How To:

<a name="how-do-i-find-the-project"></a>
#### How do I find the project? 
The code for this project is hosted in this repository. It is part of the projects hosted by the gordon-cs group, so you will need to be a part of that group and added as a collaborator on the project in order to commit changes to the repository. 

<a name="how-do-i-make-changes-and-run-the-project-to-test"></a>
#### How do I make changes and run the project to test? 
This assumes you have a working understanding of the basics of git and the way github works. If you don’t, we suggest you start there before moving forward.

This project is written in C# using the ASP.NET framework. We are using the 4.6 version of the .NET framework, so the project can only be run on a windows machine with Visual Studio. If you are reading this, chances are that you have been given access to a Virtual Machine to do development work on the project.

- Clone the project from the github repository. This should give you all the code you need to work on the project. 
- Start up Visual Studio. Open Project => Navigate to the folder you just cloned => Select the “Phoenix.sln” file. 
- When the project finishes loading, you should see the project files and folders on the right. 
- Make whatever changes you want, save and build.
- Start debugging by clicking on the Green play button on the top, or Debug => Start Debugging.
- After a few moments, the bottom status bar should turn orange, and the browser should start.

<a name="how-do-i-publish-my-changes-to-the-live-websites"></a>
#### How do I publish my changes to the live websites? 
There are two live websites accessible from within the Gordon network. You need to be connected to GordonNET to access them.

This is an example of the workflow:
- Make a change and test it locally by running it directly from Visual Studio
- Once you are confident that it is working, publish it to rcitrain.gordon.edu (See notes on how to publish below).
- As the name might suggest, rcitrain.gordon.edu is the development site. Use it to make sure the changes work as expected on the web
- Once you are sure everything works on rcitrain.gordon.edu, publish to rci.gordon.edu

Publish profiles have been set up to make publishing hassle free. When you publish to rcitrain.gordon.edu, the published web application will access the RCITrain database. When you publish to rci.gordon.edu, the published web applicatoin will access the main RCI database. This should happen automatically thanks to the publish profiles.

To publish:
- Open Visual Studio as **an administrator**.
- At the top, click on Build => Publish Phoenix.
- A Publish Web window will open. You will find a drop down at the top with two options: RCI_Publish_DEV and RCI_Publish
- Select RCI_Publish_DEV and click publish to publish to rcitrain.gordon.edu
- Select RCI_Publish and click publish to publish to rci.gordon.edu

<a name="how-do-I-change-the-format-of-the-fine-email"></a>
#### How do I change the format of the fine email? 

Ok, so you have set up shop and received your first task; “Change the format of the fine emails to: so and so”. Your first questions might be: What are fine emais? Where are they and how can I change them? No worries, we anticipated this and got you covered.

When a resident checks out of their room with their Resident Advisor (RA), the Resident Director (RD) and RA eventually walk through the room a second time to finalize charges. Once that is done, the system sends out an automated email to the resident (or residents if it is an apartment common area) with a listing of the charges they owe. The format of the email is pretty much set, but who knows, maybe they want to add a comma somewhere in there.

Open up the project and select the Phoenix node on the right. Right-click on it and select Properties, then Resources. You should see a table with resources the project has. We are using the ASP.NET Resource feature to store the fine email string. This is not necessarily the best way to do it, but that’s what we came up with at the moment. 

You should be able to edit the email string in this window. A few things to note:

Use the arrow keys to navigate across multiple lines, trying to click doesn't work as expected in this window.
When the email string is used, it is formatted to include dynamic information like the date, the name of the resident, description of charges etc… This information ultimately appears where you see the {SOME DIGIT} symbols. If you are going to edit the fine email extensively, you should probably read more about C# string formatting before doing so. We currently don't do any complex string formatting, so you don't have to read much to understand how it works. The first paragraph of this article should bring you up to speed:

http://azuliadesigns.com/string-formatting-examples/



Point of imporvement: Allow Admins to change the fine email from the web portal.|
--------------------------------------------------------------------------------|


<a name="how-do-i-manually-change-the-furniture-that-rcis-have"></a>
#### How do I manually change the furniture that RCIs have? 

RCIs come by default with a set of furniture to which you can add damages. It is possible to change that. One of the Admins should be capable of doing this through the web portal, so there should never be a need for you to do it manually. However, we will talk about this here to help introduce you to the different parts of the project.

Each dorm has a different set of furniture and the system reflects this. We use an SQL table as a template to tell the system what furniture needs to be generated for each type of room. The table in question is the `RciTypeRoomComponnentTypeMap` As the convoluted name suggest, this table maps each type of rci to the different room component types it will have.

There are currently two Rci Types (as of August 4th 2018): Common Area and Dorm Room.  
A Room Component is a fancy name for "Furniture". It also refers to more general things like Incomplete checkout and Lost key fins that are "attached" to an Rci.

To add a new Room Component Type to Ferring Dorm Room Rcis, you'll need to add a new row in the `RciTypeRoomComponentTypeMap` table. 


<a name="how-do-i-find-user-uploaded-pictures"></a>
#### How do I find the user-uploaded pictures? 

A better question is: What actually happens when you publish the project?

When you type in “rci.gordon.edu” you are actually accessing files on some remote computer. Specifically, you are accessing files on the CS-RDP-1 Virtual machine, which might be the machine you are using to work on the project. It should come as no surprise then, that you can find the published product on the machine you are using (if you are using CS-RDP-1 to do you development work). Navigate to the `F:\Sites` and you should see folders for all the sites that are hosted on this machine. There should folders for rci and rcitrain. These correspond to the sites rci.gordon.edu and rcitrain.gordon.edu. The pictures uploaded by users should appear under Content\Images. Images are organized into folders according to date of upload to make navigating easier.

<a name="how-do-i-manually-query-the-database"></a>
#### How do I manually query the database? 
As you start working in earnest on the project, it will often prove useful to examine the database or update it directly via queries. This can also be done using Microsoft Sql Server Management Studio. Open it up and log in to the `adminprodsql.gordon.edu` server using Windows authentication.
"Windows Authentication" means "Use the same credentials I used to log into Windows".

Both the RCI database or the RCITrain database exist on this server. I would stay away from querying the RCI database completely since it contains real user data (unless you know what you are doing). 
Naturally, you will need to be familiar with some SQL query the database, and as this is not an SQL tutorial, you will find no help with that here...


### Deep Dive

#### The Database
A distinction must be made between our tables and the views that were provided to us by CTS. Views are basically non-editable tables. The Views can be found under….you guessed it: the Views folder in either the RCITrain or RCI databases. The Tables can be found in the Tables folder. 
If you find something is missing in a View, it is most likely a CTS person that will have to update it (remember you can’t update it, you can just read).

Important: Both RCI and RCITrain databases must have IDENTICAL schemas at all times. So if you make a change to the schema of one, make sure you do it for the other.|
---------------------------------------|


_Views:_
Account: Account information for everyone at the college. Does not contain password info.

CurrentRA: The canonical view for which accounts are RAs. If this isn’t up to date, the system won’t be either.

CurrentRD: The canonical view for which accounts are RDs. If this isn’t up to date, the system won’t be either.

Room: A view with all the rooms on campus. We have currently restricted it to only show residential rooms, but it actually also has access to all types of rooms including classrooms and offices. Those are not needed for the purposes of this system though.

RoomAssign: This is an important view. It dictates which resident is assigned to which room. The system depends on this to generate rcis. If a resident does not have a room assignment in this view, one must be created by the Residence Life staff before the resident will see their rci.

Session: Another important view. It is basically a listing of the all sessions present and past. Rci generation depends on this.

RoomChangeHistory: We don’t use this (^_^;)

_Tables:_
These are tables that we created. You have full control over these.

Admin: A listing of the id numbers of the Admins. If someone is listed here, their admin role takes precedence over everything else. E.g. If you want to test the admin functionality, you can add your id number here...of course, this should be done on the RCITrain database 

BuildingAssign: A mapping of Job Titles to hall codes for the halls a given RD is responsible for. If I am the RD of “Tavilla and Bromley”, there will be two records in the table that are as follows:
“Tavilla and Bromley” : TAV
“Tavilla and Bromley” : BRO 

CommonAreaSignatures: A table to store common area signatures for common area rcis. A signature is either for Checkout or for Checkin.

Damage: This it the table that stores the damages that users enter. The type can be either TEXT or IMAGE. (WE NEED TO REMOVE THE FINE ASSESSED COLUMN)

Fine: This is the table that stores the charges and fines that RAs and RDs enter for residents.

Rci: This table represents the Rci entity. It has columns for all the Checkin and Checkout signatures so we can keep track of the state of the Rci.

The IsCurrent column is used to differentiate active rcis from old rcis. An rci is active throughout the year until sometime after checkout when the RD decides that they don’t need it anymore. They can then archive it by making it non-current
If an RCI has a GordonID, it is a normal rci. If it’s GordonID column is NULL, it is a common area rci.|
-------------------------------------------------------------------------------------------------------|

RoomComponentType: In the spirit of the paper rci, each rci is made up of different components. These components can then be associated with damages through the Damage table.

RciType: The different types of Rcis we might have

RciTypeRoomComponentTypeMap: A mappipng table between the RciType and RoomComponentType tables.

_Stored Procedures:_

We only have one stored procedure. Normally, we would try to keep business logic outside the database, but this query was too complex to write through c#. This stored procedure is used to determine if any new rcis need to be created. A more in-depth explanation will be given in the Rci Generation section.

#### Authentication
Authentication is the process of determining that the user has a valid identity. To help us determine validity, Gordon maintains a directory of all its users and resources. This directory is accessible via the LDAP (Lightweight Directory Access Protocol).

_So how does this happen in our application?_
When the user gets to the login page and inputs some credentials, the credentials are sent via LDAP to Gordon’s user directory. Given a username and a password, the Gordon directory can determine if the user is real and their password is correct. The result of this validation is sent back to our application. If the user is validated, a JWT (Json Web Token) is created for the user and stored in a cookie.
You can read about JWTs [here](https://jwt.io/introduction/). In short, these tokens are a secure way to reliably establish the identity of the user on the server on each request.

_Where is the code that does this?_
Good question. Controllers > LoginController.cs is the file that handles everything authentication.

The first method called “Index” returns the Login page. Notice that it is annotated with [HttpGet]. This indicates that the method is called when a request for /Index is made using the GET http verb. Whenever you enter a url into a browser, you are making a GET request to that address. So you can imagine that this method is responsible for responding to the request “GET rci.gordon.edu/Login/Index”. The default annotation for methods is [HttpGet], so if you don’t see it, assume that’s what it is.

The next method “Authenticate” is annotated with [HttpPost]. This method is responsible for accepting input from the user via a form submit. When the user submits their username and password, they get sent to this method. This is the method that does the actual authenticating. Go through it and try to understand what it does. It makes use of the LoginService class which has helper methods for authenticating. 

#### Controllers and Views
Controllers and Views are at the center of the ASP.NET MVC framework. Controllers contain the “endpoints” of your application, the accessible url routes. If your controller was called HomeController.cs and had a method called Method1, the url route to access that method would be /Home/Method1. So if you application was running on rci.gordon.edu, the full url would be “rci.gordon.edu/Home/Method1”. 

Views represent the html that the user ends up seeing in their browser.

There is no need to go deeper into Controllers and Views here. Microsoft has good documentation on starting out with their ASP.NET framework. If you are not familiar with ASP.NET at all, you should start by reading their guides and walking through their tutorials. Use these search terms to get you started: “ASP.NET MVC 5”.
Here are some links to get you started:

https://www.asp.net/mvc

https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/getting-started

https://mva.microsoft.com/en-US/training-courses/introduction-to-asp-net-mvc-8322?l=VZ2619Zy_8804984382


#### RCI Generation

How are RCI’s generated? That’s a big question, and we’ll try to break it down here:
RCIs are represented first as rows in the database. In ASP.NET, the table of RCIs is interpreted as a list of RCI objects with various properties.  
The goal is to detect when a new RCI is needed, create a new RCI object, and save it to the database. 

_Detect when a new RCI is needed:_

A resident needs a new RCI in two cases: 
They’ve never had an RCI.
The date on their most recent room assignment is newer than the creation date of their most recent rci. This means that they have moved rooms since the last time an RCI was created.

_Create a new RCI Object:_

This is a simple step. Once we detect that someone needs a new RCI, we create a new Rci object in memory and populate it with the appropriate values.

_Save to the Database:_

Send an SQL query to the database. In this case, it will be something like `insert into RCI(column1, column2, etc...) values (value1, value2, etc...)`

_Code please?_

RCI Generation happens in the DashboardController.cs file. Depending on who logs in, different things happen:

If a resident logs in, the system will detect if a new rci is needed for that specific user.
If an RA/RD logs in, a stored procedure is run that returns a list of new room assignments for which no rci has been created.
RCIs are then created for all of them.


#### Unit Tests

Testing is important for peace of mind.  
The most important part of our application is the part that reads and writes data from/to the database. That logic is all in one place, specifically the `DapperDal.cs` file. 
Unit tests have been written for every single function in this file. You can find the tests in the `Phoenix.UnitTests` project.  
Please make sure that if you add a new function, you also add a corresponding unit test.  
I use the term `unit test` loosely here. Historically, unit tests are supposed to only test a single function in a very controlled way. No network requests, no database accesses.  
Our `unit tests` actually hit the RCITrain database. My thought process was that "If I am testing the Data Access Layer, then I better test that the data is accessed properly".  
As a consequence, you'll notice that after every test that creates something new in the database, there is a cleanup section to clear all the things that test created. Do you best to follow the same template.


#### Querying the Database

We were not always making raw SQL queries to the database. We actulally started out using Microsoft's Entity Framework ORM. Once setup, it makes accessing the database intiuitive. The syntax for accessing rows in a table resembles that of accessing a C# list for example. This is all well and good until you need to modify the database in some manner.  
I never got around to really mastering Entity Framework. Making schema changes was always a hassle. I doubt any future maintainers working on this project would want to deal with Entity Framework as well.  
So, I switched to using Dapper. Dapper does much less than Entity Framework does and requires 0 knowledge other than C# and SQL. You write your sql queries, send them to the database and get your results back. It is as simple as that. No weird setup. No hours wasted trying to figure out Entity Framework migrations (I still don't understand what these are .__.). 


#### Exception Handling

- In the Data Access Layer (DAL): If I know a specific type of exception might be thrown (e.g. For methods that expect exactly one result and use `.Single()`, I catch `InvalidOperationException`, which will be thrown when there are either many or no results), I'll catch it and deal with it. Dealing with it means (a) performing some other action that will fix the problem or (b) wrapping the exception in a custom exception (e.g. UserNotFoundException) and throwing that.
- In the Services: If I know a DAL method can throw a custom Exception (e.g UserNotFoundException), I catch that, log and either throw or fix the issue if possible. I also always wrap EVERY call in a try/catch for the general exception type, log it and rethrow it.
- Controllers: I don't think I do any exception handling here.
- ExceptionFilter: Logs any unhandled exceptions (in case something was missed)

*The result is that there are two types of errors*:  
(1) Error cases I KNOW about. These are caught using Custom Exceptions and handled correctly.  
(2) Error cases I DON'T KNOW about. I log these and let the application error out. The logs will give me enough information to identify the error and possibly fix it, thus making it part of (1). 





