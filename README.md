# API-for-components-management 


## Purpose behind the project 
I started this project because I was doing a stage in a company near where I live. I was in charge of creating the API and the database, as you may have guessed from the name, the project main purpose is to interact with the database by using this API, and then to send the information to the website. 

## The database
The DB is structured with three main tables:
* ComponentsDA: The first one and the main one, contains all the information about the components (Ex. Engine, accelerometer, strain gauge).

![Components DA](https://github.com/Pablososs/api-for-components-management/assets/134268303/2e8a0781-9a25-40b3-b6a2-ce368dd856bd)

* Details: This one is connected to ComponentsDA with a relationship type (1:N) ; this table contain the information about the components so basically all the stuff that are useful to track trend of our machines (Ex. Engine = revolutions of the engine, temperature, electricity consumption).


![Details](https://github.com/Pablososs/api-for-components-management/assets/134268303/d9c3a7e4-16b9-4337-b573-eb8ac8f45076)


* Positions: The last one is used for purposes of the website, it contains the information about the positions of the button in the Image of the machines, for more info click on [Link.](https://github.com/LucaGiovannini02/details-by-a-photo)

![Positions](https://github.com/Pablososs/api-for-components-management/assets/134268303/586dbbc8-3ae1-485d-96ee-e08e3b95de08)

## The API
The API are all coded in C# with .NET Framework, I have also used Swagger to test this code. All this api are basically made to execute Query(Ex. Delete, Update, Select), I have divided all the API in two categories, in the first you can see all the controllers , and in the second one you see all the classes for each table.

## Final words
I wanted to especially thank the company that gave me an opportunity to try my hand on this type of work, and also a special thanks to [LucaGiovannini02](https://github.com/LucaGiovannini02) that worked on the front-end side, so please go and check out his work. 
* [Luca's first linked project](https://github.com/LucaGiovannini02/components-details) 
* [Luca's second linked project](https://github.com/LucaGiovannini02/details-by-a-photo)
