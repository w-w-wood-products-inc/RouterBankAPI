using demoWebAPI.models;
using FirstAPI.Services.Interfaces;

namespace FirstAPI.Services; 

public class UserAccountManager : IUserAccountManager {

    private List<UserAccount> _userAccounts { get; }

    public UserAccountManager() {
        _userAccounts = new List<UserAccount>();
    }

    public List<UserAccount> GetUsers() {
        return _userAccounts;
    }
    
    public UserAccount? FindUserById(int userId) {
        return _userAccounts.Find(userAccount => userAccount.userId == userId);
    }

    public bool AddUser(UserAccount user) {
        if (FindUserById(user.userId) == null) {
            _userAccounts.Add(user);
            return true;
        }
        else {
            return false;
        }
    }

    public bool DeleteUser(int userId) {
        int index = _userAccounts.FindIndex(userAccount => userAccount.userId == userId);
        
        if (index == -1) {
            return false;
        }

        _userAccounts.Remove(_userAccounts[index]);
        return true;
    }

    public bool ModifyUser(UserAccount user) {
        int index = _userAccounts.FindIndex(userAccount => userAccount.userId == user.userId);

        if (index == -1) {
            return false;
        }
        
        // Modifies all values of the user account except userId
        _userAccounts[index].name = user.name;
        _userAccounts[index].username = user.username;
        _userAccounts[index].savingsBalance = user.savingsBalance;
        _userAccounts[index].checkingBalance = user.checkingBalance;
        _userAccounts[index].minutePercentageRate = user.minutePercentageRate;
        _userAccounts[index].mprEnable = user.mprEnable;
        return true;
    }
}