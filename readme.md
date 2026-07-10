# LegacyOrderService
Legacy order service that needs a good refactor.

# Requirements

This small C# (.NET 8) console application was built to meet immediate needs, but the business anticipates substantial growth. The codebase will need to 

1. scale to support new features, 
1. higher throughput, and 
1. possible system integrations.

You must
1. Identify and fix bugs or runtime issues
1. Refactor poor architecture and code smells
1. Apply appropriate design patterns and modern C# best practices
1. Improve performance, resilience, scalability and testability
1. Make decisions based on real-world engineering tradeoffs

(Some definitions, in case you do not know:
Resilience = the ability to spring back to an original form after having been squeezed, stretched, etc.; the ability to recover quickly from illness, misfortune, troubles, or the like.
Scalability = the ability of something, esp a computer system, to adapt to increased demands
)

# Branches
* `development` this branch has the latest changes. It always compiles.
* `dev/123_branch_name` All development work goes under `dev`. The branch name should start with a Github issue number followed by an underscore and then a short description of the branch in snake_case.

# Ideas for Refactoring and Improvements.

brainstorming list of todos...

1. Add unit tests.
1. Reduce coupling.
1. Use asynchronous database operations.
1. Refactor into layers (domain, application, infrastructure, presentation)
