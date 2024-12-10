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

   
<div style="text-align: center;">

  <h3>Home Page</h3>
  
   - The home page consists of a navigation bar with three buttons, logo and name, SVG animation of a truck moving, flip cards, 
   "Meet Our Partners" section, "Our Achievements" section, and a footer with links, company information, social links and contact information.

   <p align="center">
   <img src="./documentation_images/home-page.png">
   </p>
</div>
