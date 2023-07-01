# Assignment #5

## C&#35; - Kanban Board part trois

[![Simple-kanban-board-](https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Simple-kanban-board-.jpg/512px-Simple-kanban-board-.jpg)](https://commons.wikimedia.org/wiki/File:Simple-kanban-board-.jpg "Jeff.lasovski [CC BY-SA 3.0 (https://creativecommons.org/licenses/by-sa/3.0)], via Wikimedia Commons")

Fork this repository and implement the code required for the assignments below.

### Prequel

Inspect the code for `TagRepository`, `TagRepositoryTests`, `TaskRepository`, and `TaskRepositoryTests`. They contain a sample solution for last weeks assignment.

### Exercise 1

1. Implement and test the `IUserRepository` interface.

```csharp
public interface IUserRepository
{
    Task<(Response response, int userId)> CreateAsync(UserCreateDTO user);
    ICollection<UserDTO> Read();
    Task<UserDTO> ReadAsync(int userId);
    Task<Response> UpdateAsync(UserUpdateDTO user);
    Task<Response> DeleteAsync(int userId, bool force = false);
}
```

with the following rules:

- A user who has tasks assigned to them cannot be deleted, except if using the `force`.
- Trying to delete a user who has a task without using the `force` should return `Conflict`.

Your code must use an in-memory database for testing.

### Exercise 2

Make the `ITaskRespository` `async`. Ensure that all tests are updated as well.

### Exercise 3

Make the `ITagRespository` `async`. Ensure that all tests are updated as well.

### Exercise 4

Using the `ParallelOperations` class do the following:

Test and implement the `Squares` method with the following specification:

- It should return a collection of the squares from `lowerBound` to `upperBound`).
- Example: given `1` and `5` it should return `[1, 4, 9, 16, 25]` (need not be sorted)
- Computation must be done in parallel using a thread-safe collection to hold the calculated values.
- You should use the `Parallel` class.
- *Bonus*: Implement a version which returns the squares in ascending order.

Test and implement the `SquaresLinq` method with the following specification:

- It should return a `count` of the squares from `start`.
- Example: given `1` and `5` it should return `[1, 4, 9, 16, 25]` (need not be sorted)
- It should support all integers - `int.MaxValue^2 = 4611686014132420609` and `int.MinValue^2 = 4611686018427387904`.
- Computation must be done in parallel using a single LINQ query.
- *Bonus*: Implement a version which returns the squares in ascending order.

Test and implement the `CreateThumbnails` method with the following specification:

- It should create a thumbnail for each of the supplied `imageFiles` and save them in the `outputFolder`.
- Image files must be at most of `size`.
- Computation must be done in parallel.
- Testing must *verify* that the `resizer` was called with the right parameters.


## Software Engineering

Note: the two following exercises could be used in class to further discuss additional examples of the SOLID principles.  If you wish to present and discuss your examples discussed in class, I would be happy to include an hour in one of the future lectures.  Therefore, if the above sounds interesting, please do not hesitate to contact me.  The idea would be to have a brief slot in which you would present your example of one or more of the SOLID principles.

### Exercise 1

For each of the SOLID principles, provide an example through either a UML diagram or a code listing that showcases the violation of the specific principle.
_Note:_ the examples do not need to be sophisticated.

### Exercise 2

For each of the examples provided for SE-1, provide a refactored solution as either a UML diagram or a code listing that showcases a possible solution respecting the principle violeted.
_Note:_ the examples do not need to be sophisticated.
