namespace ContractPortal.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContractPortal.Models;
using WebApi.Helpers;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(string id);
}

public class UserService : IUserService
{
    private readonly AppSettings _appSettings;

    public UserService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
    private List<User> _users = new List<User>
    {
        new User { Id = "0001", Username = "user1", Password = "password1",  
            PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\nMIICXQIBAAKBgQDiS5F+PgNCzj6iETZ13AlWw23yroAtMeALFmeZGEOg3+WhLU6uKUryWWg+Vzrqowp4F99rAsdXs4EKLpYVzUPRE73c+gWZz9x84qGWaqxK5o+z6VkK7KcbAgLN6zb555kVFq04fafZqkmJadoyXleLYjEqcJ7W6UteWoLs0Qy7YwIDAQABAoGAUG3lZ0YpKIxfTIDrp1YuZ40MPe3xlp6cb7Rl28748mvBlNiil1oLzjkiyM1+HjkWlnp9qO4S5cPiADlwlI0hJbpNFRf8Kqu00CHWseHrfk7yhyUrOoxjSKn6xVw6fq7I50GoqHz9UmnxFBXGisGmUliH01uTHLNqBcIpJ1v+FxECQQDskBUboEg/ArXMwLEMpptTONDIAAA80ZhQSZ5M9rYKbzPPQTypc8+bVCZ9zJ60iJOJ3S27upNl/QBNvdEH6dsLAkEA9OODm0CdR7+AWAryynMaoOGj7XrZ9aLVRf2NwE0MRXH/rB32B6P7vCmJIyyTMElV6eku80/mo1XT9tKyrsEYCQJBAOi8lFe6qHl9lBkeltGodHY7FoU+Iv2zA5Qx6ZE0xEKdxy4ns6PPMbhS4Q+xKY7aM7VWKnFgjTWw5QSXNDkB5aMCQCw1UlHZpUsJiCrctx3TD7CRa114uxY78hJzhn57qkZzIPu6YOraMJy0RtyBtISYCJl0jhRAjVtZKC27taQUmbkCQQCFOFCDlgGoOpNBY763l/12hnaWE7++iLi6D8ysseRE7aXsufvHOVNQiACYDM65MrbM3HwmzpH+uQhmtX/HZ1kl\n-----END RSA PRIVATE KEY-----",
            PublicKey = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDiS5F+PgNCzj6iETZ13AlWw23yroAtMeALFmeZGEOg3+WhLU6uKUryWWg+Vzrqowp4F99rAsdXs4EKLpYVzUPRE73c+gWZz9x84qGWaqxK5o+z6VkK7KcbAgLN6zb555kVFq04fafZqkmJadoyXleLYjEqcJ7W6UteWoLs0Qy7YwIDAQAB\n-----END PUBLIC KEY-----"},

        new User { Id = "0002", Username = "user2", Password = "password2",
            PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\nMIICXAIBAAKBgQDcCh461jlglt/5kSa+7VLf+KLv2BpxfE5mAftIhtJGgoD0kI2EFIy5env5t/2KdY4mylRIAOZctBS0J+qpQm79cQBaNcKzYZXmf646/SEyEl49/Fil2Sf/GDTLhXzthQ9VYf2CH2uhFKHjvtH1FT1Xqvts91Osyivi4HIKPcwozQIDAQABAoGAGhWU+dKVYK7iBDrBxhZqIaTe4+HRWKysuoj3meRpnvQvh05viD3LZN1xPuwhwT0oSc7oaNS6ejjaz3R/6+q1L1vNPMijdAAaXodSXFaUwjy4hSp3MaOsjlw/xnHEfrVhunlvUX1zCS4qM+W2+t0yIXkklmc0XUWdtxvk0gWiyP0CQQDf8RhLF+9N8CaJjZqf9MzzGLtNMRI0xZ3KFKzkJd/DyjLlFddxwQtAvMsALohsNve0uok+HwAmD2crSTYfMrzjAkEA+4oD8GSO6F639uOUG6E2xs4IPKtYoWEYBwtvrEgtSyhXAdaIz9pHO1mT/GS7T0Pn+gIIeKDnfjetYN83v++ijwJATOkUmtXBjlboJV3IuR7uNJ9N1JkB07X7Fyg6qLCJRUjwadUyhRLztmwgnBNfLytBr4RRFlC3UFsB58/hEp3uWQJAJvIIiBjZs4quyxATZ2+/jmpqBhK1h08ELFsN8EvAj4lwZz1zM1Nlgf/tlL937N7jtObJrvuiu/0c4AnPlO0iawJBAMhVnELvykSCf1jdPVrVdgn8sdHdefF6MykZLhn30pYJhGD4a1gF7+DzXBAsqt/1wXjagTzmRLcd59ORHbZ7tZ4=\n-----END RSA PRIVATE KEY-----",
            PublicKey = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDcCh461jlglt/5kSa+7VLf+KLv2BpxfE5mAftIhtJGgoD0kI2EFIy5env5t/2KdY4mylRIAOZctBS0J+qpQm79cQBaNcKzYZXmf646/SEyEl49/Fil2Sf/GDTLhXzthQ9VYf2CH2uhFKHjvtH1FT1Xqvts91Osyivi4HIKPcwozQIDAQAB\n-----END PUBLIC KEY-----" }
    };

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

        // return null if user not found
        if (user == null) return null;

        // authentication successful so generate jwt token
        var token = generateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public User GetById(string id)
    {
        return _users.FirstOrDefault(x => x.Id == id);
    }

    // helper methods

    private string generateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}