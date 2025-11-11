# Finviz.TestApp

## Tech-stack

- C# 13 & .NET 9.0 (`https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview`)
- Typescript 5.9.3 & React 19.2.0 (`https://react.dev/blog/2025/10/01/react-19-2`)
- Dapper 2.1.66 (`https://github.com/DapperLib/Dapper`)
- Serilog (`https://serilog.net`)
- PostgreSQL (`https://www.postgresql.org`)

## Clean Architecture

Clean Architecture is a software design philosophy that separates the software into layers.
Each layer has a specific responsibility and each layer is independent of the other layers.
The layers are:

- Domain: This layer contains the business logic of the application. It is independent of any other layer.
- Application / Use Cases: This layer contains the application-specific business rules. It is independent of the delivery mechanism.
- Persistence: This layer contains the database access code. It is independent of the application and the delivery mechanism.
- Presentation: This layer contains the delivery mechanism.

## Services

| Service                         | Dev URL                 | Docker Ports |
|---------------------------------|-------------------------|--------------|
| **Finviz.TestApp.ImageNet.Api** | https://localhost:7094/ | 31337:8080   |
| **Finviz.TestApp.Web**          | http://localhost:5173/  | 5173:80      |

## Managing docker and user secrets

For setting up the secrets values use:
- `.env` file from solution folder. Manually copy the sample file with removing of `.sample` extension (please, don't remove the sample file) and set your values (username, password, etc.) by example template.

## Local development
- Run `CreateAppStack.sh` script from `Finviz.TestApp` root folder. Three new docker containers inside `finviz-test-app` label should run.
- Go to `http://localhost:5173/` in your browser and start from impoting the data.
  - The data source is hardcoded in the current version of the application.
  - Used the XML file from: `https://raw.githubusercontent.com/tzutalin/ImageNet_Utils/refs/heads/master/detection_eval_tools/structure_released.xml`

## Bulding tree algorithm
For checking bulding tree algorithm, please call http://localhost:31337/api/imagenet/tree
The code is located in `Finviz.TestApp.ImageNet.Domain.Entries.ImageNetService`

Some notes about the algorithm:

```aiignore
The method BuildTree reconstructs a hierarchical taxonomy of ImageNet entries from a flat list retrieved from the database.
It uses a dictionary for constant-time parent lookups, ensuring the algorithm runs efficiently even on large datasets.

Algorithm Description
1. Iterate once through all entries and create a lookup dictionary:
   Dictionary<id, entry> → enables O(1) parent access.
2. Iterate again to assign each node to its parent (if any).
3. Collect all root nodes (entries without a parent).

| Type  | Complexity | Explanation                                                                           |
| ----- | ---------- | ------------------------------------------------------------------------------------- |
| Time  |    O(n)    | Each node is visited exactly once. Lookup and insertion operations are constant time. |
| Space |    O(n)    | Additional memory is used for the dictionary that stores all entries.                 |

```

## Conventional commits

Please follow the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification when creating commit messages.

The pattern is as follows (please omit the < and > characters):
```
<type>(<scope>): [<ticket-number>] <description>
```
