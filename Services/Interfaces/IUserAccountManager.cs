using demoWebAPI.models;

namespace FirstAPI.Services.Interfaces; 

public interface IUserAccountManager {
    
    private List<UserAccount> _userAccounts {
        get { return _userAccounts; }
    }

    // Description: Returns all users in the list
    // Returns: A list of all users
    public List<UserAccount> GetUsers();
    
    // Description: Finds a user in the list and returns them
    // Returns: A UserAccount if the specified user is found and null otherwise
    public UserAccount? FindUserById(int userId);
    
    // Description: Adds a user to the list if the userId is not taken yet
    // Returns: true if a user was added to the list successfully and false otherwise
    public bool AddUser(UserAccount user);
    
    // Description: Removes a user from the list
    // Returns: true if a user was deleted successfully and false otherwise
    public bool DeleteUser(int userId);
    
    // Description: Modifies any variable except userId from a user on the list
    // Returns: true if a user was successfully modified and false otherwise
    public bool ModifyUser(UserAccount user);

}