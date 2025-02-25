# Backend for Gamified Coding Platform

Hello and welcome to the README section of this repository. Before we start, this project is a backend system for a gamified coding platform that aims to motivate engineers to solve coding challenges and tasks through gamification. The frontend part of the project is located in a separate repository.

---

## Overview

The backend is built using .NET C# with a microservices architecture, while the frontend portion is being developed in the game engine Unity. To draw a simple comparison, this project is similar to platforms like LeetCode but takes more of a gamified approach by integrating coding challenges into a gaming-oriented environment.  

In short, it provides users with an interface where they are presented with a coding task and a code editor. Users can submit their solutions to these tasks, and that is where this project comes into play. **Its main purpose is to compile, execute, and evaluate user-submitted code.**  

## Architecture, Features & Technologies

The backend consists of several microservices, each responsible for a different part of the system.  

**Why microservices?**  
Mainly for modularity and separation of concerns when it comes to adding new features and modules, such as support for different programming languages in the future.  

##### **Python Code Execution & Evaluation Microservice**

- Executes Python code using the `Process` class  
- Injects testing data into the user’s code and runs it  
- Compares the output with the expected result and determines if the solution is correct  
- Returns a structured response with a success/failure status and a feedback message explaining the result

##### **Task Provider Microservice**

- Delivers coding tasks to the frontend when requested
- Communicates with language services (currently only Python) to provide coding templates for users  

##### **API Gateway**

- Acts as the entry point for frontend requests
- Routes API calls to the appropriate microservices

##### **MongoDB Database (NoSQL)**

- Stores all task-related data:
  
  - Coding tasks
  - Testing data for each task
  - Coding templates for different programming languages

- **Why NoSQL?**  
  The coding tasks contain nested and inconsistent data structures. For example, the coding tasks might have a different number of input arguments.  

### **Docker & Containerization**

The entire backend is Dockerized, running multiple containers:  

- API Gateway  
- Python Service  
- Task Provider  
- MongoDB Database  

Containers communicate internally and the Docker Compose file manages networking, ports, and dependencies.  

---

## Future Endeavors

In this section, I’d like to include a rough roadmap, mainly for the backend part of the coding platform.  

- Implementing a message broker like **RabbitMQ / MassTransit** to limit the number of requests that are being processed concurrently.  
- Logging and monitoring via **Seq**.  
- General improvement of security when processing user-submitted code.  
- Authorization and authentication.  
- Unit and integration testing.  

## Frontend (Unity)

While this README focuses on the backend, I’d like to briefly touch upon the user interface, which consists of three main panels:  

- **Task Panel** – Displays the coding task  
- **Result Panel** – Shows feedback and results after code execution  
- **Code Editor** – Where users write and submit their code  

Later down the line, this user interface will be integrated into a gaming environment that rewards users for completing coding tasks.  

---

In conclusion, I’m aiming to make this project as flexible and modular as possible. While the current frontend is being developed in **Unity** with a game-oriented experience in mind, this backend is not tied to any specific interface. It can be integrated with various frontends, whether it's a web-based coding platform, a mobile application, or an educational tool.  At least that’s the idea. 😃  
