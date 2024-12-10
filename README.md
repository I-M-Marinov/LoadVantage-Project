<div style="text-align: center;">

 <p align="center">
   <img src="./documentation_images/loadVantage-logo.png">
   </p>
  <h2>
    LoadVantage Web Project Application:
  </h2>

  <ul>
   <p> This web application was created as a final project in The Software University's ASP.NET Advanced Course 2024. It is a type of multiservice monolith. The application itself represents a load board web application that users ( Brokerage Firms and Truck Load Carriers ) can use to connect and do business. Users can be two types Brokers and Dispatchers. Brokers can create loads, post the them on the loadboard and on the other side a Dispatcher can book the load and assign a Driver and a Truck on it. After successful completion of the load the Dispatchers mark the load as delivered. Brokes can also unpost loads that are posted ( one or multiple ). They can cancel loads altogether ( essentially removing the load from their boards ) or cancel the Truck Load Carrier that is already assigned on the load. A Dispatcher can also cancel a load ( essentially returning the load to it's owner - the Broker that created it ).</p>

   <p> The application has two main roles --> 🔵Administrator and 🔴User. Registering an account creates only Users ( Two types of users --> Dispatcher and Broker ). 
        Administrators can either be directly seeded in the database or manually created by another Administrator using the User Management panel. </p>
    
   🔵 Administrators can manage users, manage loads, start a chat with all Users and other Administrators, view application statistics and pull pie chart reports. 
   </br>
   </br>
   🔴 Dispatchers can book posted loads on the Load Board, manage trucks in their fleet, manage drivers in their company, return booked loads back to the owner ( Broker ), message any broker that has a Posted Load.
   </br>
   </br>
   🔴 Brokers can create a load, edit a load ( in all statuses except Cancelled ), post a load, cancel a load, cancel a carrier assigned to a load (as long as they are the owner of the load ), chat with Users and Administrators ( if they are contacted first ),             </br>
   </br>

   ✅ Both Users and Administrators can view their own profiles, summary of their loads if Brokers and summary of the Booked or Delivered loads if Dispatchers and a summary of their Trucks and Drivers, they can edit the profile information, edit or remove their profile picture<sup>🌟</sup> and change their password.
    </br>
    </br>
    <sup>🌟 One difference here is that Administrators can use a .GIF image for their profile picture and Users cannot.</sup>
   
  </ul>
   
</div>

<div style="text-align: center;">
  <h2>
    Database Diagram:
  </h2>

   <p align="center">
   <img src="./documentation_images/database-diagram.png">
   </p>
</div>

<div style="text-align: center;">
  <h2>
    Technology Stack:
  </h2>
  
  <h3>Front End:</h3>
  <ul>
    <li>HTML & CSS</li>
    <li>SCSS</li>
    <li>Bootstrap 5</li>
    <li>JavaScript</li>
    <li>jQuery</li>
    <li>AJAX</li>
    <li>Scalable Vector Graphics</li>
    <li>BoxIcons & RemixIcons</li>
    <li>Simple Datatables</li>
    <li>Apex Charts</li>
    <li>Photoshop & AI Image Generators for any images</li>
  </ul>

  <h3>Back End:</h3>
  <ul>
   <li>C#</li>
   <li>.NET 8.0</li>
   <li>.NET Core</li>
   <li>ASP.NET with MVC pattern</li>
   <li>Microsoft SQL Server</li>
   <li>Entity Framework Core</li>
   <li>Html Sanitizer Library by Michael Ganss</li>
   <li>SignalR</li>
   <li>OpenCage Geocoding RESTful API (  2,500 API requests/day for testing ) </li>
   <li>OpenRouteService RESTful API ( Directions V2 - 2000 API requests/day free )</li>
   <li>Country State City RESTful API ( No limitations, but request responsibly! )</li>
   <li>Cloudinary (Cloud Image & Video Management Service)</li>
  </ul>

  <h3>Testing:</h3>
  <ul>
    <li>NUnit</li>
    <li>Moq</li>
    <li>MockQueryable</li>
    <li>Entity Framework Core InMemory</li>
  </ul>
</div>

<h3>Source Control:</h3>
  <ul>
    <li>Git / GitHub</li>
  </ul>


 <h2>
    User Guide with screenshots:
  </h2>

<details> 
    <summary><h3>Home Page</h3></summary>
<div style="text-align: center;">
 
   <p align="center">
   - The Home page consists of a navigation bar with three buttons ( Home, Register, Login ), logo and name, SVG animation of a truck moving, flip cards, 
   "Meet Our Partners" section, "Our Achievements" section, and a footer with links, company information, social links and contact information.
   </p>

   <p align="center">
   <img src="./documentation_images/home-page.png">
   </p>
</div>
</details>
<details> 
 <summary><h3>Register Page</h3></summary>
<div style="text-align: center;">
   
   <p align="center">
   - The Register page consists of a navigation bar with three buttons ( Home, Register, Login ) and a form with information needed to register a new user in the application.
   </p>

   <p align="center">
   <img src="./documentation_images/register-page.png">
   </p>
</div>
</details> 

<details>
   <summary><h3>Login Page</h3></summary>
<div style="text-align: center;">

   <p align="center">
   - The Login page consists of a navigation bar with three buttons ( Home, Register, Login ) and a form with information needed for a registered user to log in.
   </p>
   <p align="center">
   <img src="./documentation_images/login-page.png">
   </p>
</div>
</details>
<details>
 <summary><h3>Profile Page</h3></summary>
<div style="text-align: center;">
 <p>
   - After successfully logging in a User would be redirected to the Profile Page
    </br>
  - The Profile page shows a card containing the User's full name, position and company and below it depending if User is a Dispatcher or Broker there would be counts for Drivers, Trucks, Booked and Delivered Loads ( Dispatchers ) and Created, Posted, Booked and Delivered Loads ( Broker )
    </br>
  - In the tabulated container to the right of the general info card there is a Profile Overview tab, Edit Profile Tab, Edit Picture tab and Change Password Tab
    </br>
      <h6>⭐ Profile Overview tab shows the information for the currently logged in User.</h6>
      <h6>⭐ Edit Profile Tab lets the User edit any of the information visualized in the overview.</h6>
      <h6>⭐ Edit Picture tab shows the profile picture and gives the User the option to delete it ( default to the generic picture ) or upload a new one.</h6>
      <h6>⭐ Change Password Tab gives the User the option to change his password.</h6>   
 </p>

   <p align="center">
   <img src="./documentation_images/logged-in-users.png">
   </p>
   
   <p align="center">
   <img src="./documentation_images/profile-tabs.png">
   </p>
   
</div>
</details>

<details>
 <summary><h2>Broker Load Board & Loads</h2></summary>
 <details>
 <summary><h3>Broker Load Board & Viewing a Created Load</h3></summary>
<div style="text-align: center;">
 <p>
   - After a Broker opens the Load Board, he will be redirected to the Created Loads Tab, where he can view all the loads that he/she created.
    </br>
  - Search available right on the Board, that would filter the loads as the Broker types
    </br>
  - Sorting functionality available for each piece of information visualized ( for instance Broker can sort all loads by pickup state or price ascending or descending )
    </br>
  - Pagination also available right on the table, Broker can choose how many loads per page to show ( 5, 10, 15 ) or show all loads
    </br>
  - Clicking on the button on the right in the "Actions" section will take the Broker to the Load View:
    </br>
    <h6>⭐ Edit ---> This action would give the Broker access to edit the load information. Buttons "Save" and "Cancel" appear while load is being edited.</h6>
    <h6>⭐ Post ---> This action would post the load, essentially changing it's status from Created to Available. </h6>
    <h6>⭐ Cancel Load ---> This action would cancel the load, removing it entirely from the Broker's Board. </h6>
    <h6>⭐ Back to Load Board ---> This action would return the Broker back to the Load Board ( and the Created tab ). </h6>
 </p>
   
   <p align="center">
   <img src="./documentation_images/broker-created-load-view.png">
   </p>
   
   
</div>
</details>
<details>
 <summary> <h3>Broker Viewing a Posted Load</h3></summary>
<div style="text-align: center;">

 <p>
   - After a Broker posts a load, he/she will be redirected to the Load Board's Posted Loads Tab.
    </br>
  - Search, Sorting and Pagiantion is available on every tab of the Load Board. 
    </br>
  - Clicking on the button on the right in the "Actions" section will take the Broker to the Load View:
    </br>
    <h6>⭐ Edit ---> This action would give the Broker access to edit the load information. Buttons "Save" and "Cancel" appear while load is being edited.</h6>
    <h6>⭐ Unpost ---> This action would unpost the load, essentially changing it's status from Available back to Created. </h6>
    <h6>⭐ Unpost All  ---> This action would unpost all loads, that are currently in in status Available and revert them back to Created.</h6>
    <h6>⭐ Cancel Load ---> This action would cancel the load, removing it entirely from the Broker's Board. </h6>
    <h6>⭐ Back to Load Board ---> This action would return the Broker back to the Load Board ( and the Created tab ). </h6>
 </p>
 
   <p align="center">
   <img src="./documentation_images/broker-posted-load-view.png">
   </p>
  
</div>
</details>
<details>
 <summary><h3>Broker Viewing a Booked Load</h3></summary>
<div style="text-align: center;">

 <p>
   - Any loads that are booked by a Dispatcher, would be sent to the Booked Loads Tab.
    </br>
  - Search, Sorting and Pagiantion is available on every tab of the Load Board. 
    </br>
  - Clicking on the button on the right in the "Actions" section will take the Broker to the Load View:
    </br>
  - Additional details are available to the Broker for the Dispatcher if the load is Booked. When the Dispatcher that booked the load assign a driver, information for that driver would be visualized for the Broker in that same mini window as well.
    </br>
    <h6>⭐ Edit ---> This action would give the Broker access to edit the load information. Buttons "Save" and "Cancel" appear while load is being edited.</h6>
    <h6>⭐ Cancel Carrier ---> This action would cancel the carrier on the load ( and truck if there is one assigned ) and return the load back to status Posted. </h6>
    <h6>⭐ Cancel Load ---> This action would cancel the load, removing it entirely from the Broker's Board. </h6>
    <h6>⭐ Details  ---> This action toggles the info mini window on the left side containing info about the Dispatcher and Driver (if there is one assigned).</h6>
    <h6>⭐ Back to Load Board ---> This action would return the Broker back to the Load Board ( and the Created tab ). </h6>
 </p>

   <p align="center">
   <img src="./documentation_images/broker-booked-load-view.png">
   </p>
   
   
</div>
</details>

<details>
 <summary><h3>Broker Load Board Delivered Loads Tab</h3></summary>
<div style="text-align: center;">

 <p>
   - Once a load has a Driver assigned by the Dispatcher that booked the load, he can mark it as delivered. All delivered loads for a Broker go to the Delivered Tab on the Load Board.
    </br>
  - Information for the Dispatcher and Driver that finished the load is available to make it easier to sort if needed. 
    </br>
 </p>

   <p align="center">
   <img src="./documentation_images/broker-delivered-loads.png">
   </p>
   
   
</div>
</details>
</details>


