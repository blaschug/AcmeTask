# ACME students enrollment task

## Prerequisites:
- Install DotNet SDK:  [Link](https://dotnet.microsoft.com/en-us/download/dotnet?cid=getdotnetcorecli)

## Instructions

1) Clone the repo: `git clone https://github.com/blaschug/AcmeTask.git`
2) Move to the directory: `cd ./AcmeTask`
3) Execute tests: `dotnet test` you can also run `dotnet test --logger "console;verbosity=detailed"` for a more detailed execution.


## Decisions I made:
- Based on the requirement: `Create a rich domain model (although we know that currently, the program does not seem to do much)`. I decided to implement the logic within the entities. Normally, I just create anemic models and handle the logic in services/handlers. I have not seen this domain approach in real world projects but followed it based on my understanding from DDD literature.
- Dates:
  - **Age**:I decided to use DateOnly for Student instead of Age as int, to avoid  the need for updates is and also to place some logic inside the model.
  - **DateTimeOffset**: I prefer DateTimeOffset over DateTime for better flexibility and also for future possible growth of the solution.
- I usually avoid overusing exceptions, but i added a few of them here for demonstration. Also, as in future this is going to have a UI/API I believe validation in this layer should prevent invalid data from reaching the domain.
- While I wanted to use the Result pattern (which I often implement in my projects), I avoided adding additional libraries or abstractions, focusing on simplicity. I wanted to keep it as simple as possible.
- I mapped entities to DTOs to avoid exposing sensitive or unnecessary data. This is a common practice I follow.
- I didn't include auditing fields like CreatedOn or UpdatedOn to keep the codebase simple for this task.
- While I only defined interfaces without implementations, I designed the entities with EF in mind. There are no migrations or database code in this version.
- Instead of directly referencing Student and Course, I created an Enrollment entity to encapsulate the relationship. While Student could also contain Enrollments, I didnt find it necessary here. Also, Enrollments could be virtual for future lazy loading configurations.

## Questions

- What things would you have liked to do but didn't do?
  - I would have liked to explore payment logic and state management more deeply. For now, I am simply passing two objects to the `ProcessPaymentAsync` method, but this is not a real life scenario. I would like also to use MassTransit for event driven communication (example: EnrollmentCreated -> ApplyPayment? -> ProcessPayment -> PaymentSuccess?).
- What things did you do but you think could be improved or would be necessary to return to them if the project goes ahead.
  - Logging should have been added logs! Also an EnrollmentHistory list to track enrollment events.
  - Tests might not be well organized, and some edge cases may not be covered sufficiently.
  - Adding more comments throughout the codebase could improve readability, though I believe the code is currently self-explanatory.
  - Using Mediatr for handling use cases might improve structure and abstraction. A pattern to enforce type safety in requests and responses could also be useful. 

- What third-party libraries have you used and why. 
  - Im just using Xunit and Moq. As I told above, maybe I could use Mediatr or MassTransit. But i preferred to keep it simple.
 
- How much time you have invested in doing the project and what things you have had to research and what things were new to you.
  - I spent about a day and a half on the project, started late Monday. I had to refresh some knowledge related to testing, as I had been using other libraries recently.
