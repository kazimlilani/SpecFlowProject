Feature: TODOweb

visit todo website and login 

@LoginFailed
Scenario: 1 Navigate on todo list site
    Given I am on the TODO Website
    When I enter my email "kazim@outlook.com" and click submit
    And I enter my password "wrongPassword" and submit
    Then I should see error message "Failed to login"

@LoginPassed
Scenario: 2 Navigate on todo list site
    Given I am on the TODO Website
    When I enter my email "kazim@outlook.com" and click submit
    And I enter my password "kazim" and submit

@TaskAdd
Scenario: 3 Create a new task with details
  #Given I am on the task management page
  When I click on the "New Task" button
  And I enter "automation task" in the title field
  And I enter "i need complete my automation task by today" in the description field
  And I select "Medium" priority from the dropdown
  Then I click on the "Add New Task" button
  And the new task should be created successfully

  @TaskDeletion
Scenario: 4 Delete an existing task
  #Given I am on the task management page
  And there exists a task named "automation task"
  When I delete the task named "automation task"
  Then the task should be removed from the list

@API @Users
Scenario: 5 Fetch list of users from API
  Given I send a GET request to "https://reqres.in/api/users"
  Then the response status code should be 200
  And the response should contain a list of users
  And each user should have id, email, first_name, last_name and avatar fields

@API @Users @POST
Scenario: 6 Create a new user via API
  Given I prepare a new user with name "John Doe" and job "Developer"
  When I send a POST request to "https://reqres.in/api/users"
  Then the response status code should be 201
  And the response should contain the created user details
  And the response should match the request data

@API @Users @PUT
Scenario: 7 Update an existing user via API
  Given I prepare user update data with name "Jane Doe" and job "Designer"
  When I send a PUT request to "https://reqres.in/api/users/1"
  Then the response status code should be 200
  #And the response should contain the updated user details
  #And the response should match the update data
  And the response should contain the updated timestamp

@API @Users @DELETE
Scenario: 8 Delete an existing user via API
  Given I have a valid user ID "1"
  When I send a DELETE request to "https://reqres.in/api/users/1"
  Then the response status code should be 204
  And the response should be empty
