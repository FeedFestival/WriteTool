using UnityEngine;
using System.Collections;
using SQLite4Unity3d;

public class User
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProfilePicIndex { get; set; }
    public string Saying { get; set; }

    public string Email { get; set; }
    public bool IsUsingSound { get; set; }
    public int PrankCoins { get; set; }

    // Game
    public int ConnectionId;
    public bool AllreadyIn;

    public override string ToString()
    {
        return string.Format(@"[User: 
                            Id={0}, 
                            Name={1},
                            IsUsingSound={2}, 
                            ]",
                            Id,
                            Name,
                            IsUsingSound);
    }

    public User()
    {

    }
    
}
