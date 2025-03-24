using System.Collections.Generic;
using System.Linq;
using MakeupReviewApp.Models;
using MakeupReviewApp.Models.ViewModels;

namespace MakeupReviewApp.Repositories
{
    public class MockUserRepository
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, FullName = "Alice Johnson", Email = "alice@example.com", Password = "password123" },
            new User { Id = 2, FullName = "Brenda Smith", Email = "bob@example.com", Password = "securepass" },
            new User { Id = 3, FullName = "Aless", Email = "alesstongwen@gmail.com", Password = "123456" }
        };

        private static List<UserProfile> _userProfiles = new List<UserProfile>
        {
            new UserProfile { User = _users[0], ProfilePicture = "/images/alice-profile.png", JoinDate = DateTime.Now.AddYears(-2) },
            new UserProfile { User = _users[1], ProfilePicture = "/images/bob-profile.png", JoinDate = DateTime.Now.AddYears(-1) },
            new UserProfile { User = _users[2], ProfilePicture = "https://media.istockphoto.com/id/1135151062/photo/beagle-tired-sleeping-on-couch-yawning.jpg?s=612x612&w=0&k=20&c=dpTxQw8j0btYXEj0WSIVtQT5BuBSlABs_BjD6lJxang=", JoinDate = DateTime.Now.AddMonths(-6) }
        };

        private readonly MockReviewRepository _reviewRepo;

        // ✅ Inject MockReviewRepository instead of creating a new instance
        public MockUserRepository(MockReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public User? ValidateUser(string email, string password)
        {
            return _users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public User? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetUserByName(string fullName)
        {
            return _users.FirstOrDefault(u => u.FullName == fullName);
        }

        public UserProfile? GetUserProfileByEmail(string email)
        {
            var user = GetUserByEmail(email);
            if (user == null) return null; // Ensure user exists

            var profile = _userProfiles.FirstOrDefault(up => up.User.Email == email);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    User = user,
                    ProfilePicture = "/images/default-profile.png",
                    JoinDate = DateTime.Now.AddYears(-1),
                    Reviews = _reviewRepo.GetReviewsByUser(user.FullName)
                };
                _userProfiles.Add(profile); // ✅ Automatically add the profile
            }
            else
            {
                profile.Reviews = _reviewRepo.GetReviewsByUser(user.FullName);
            }

            return profile;
        }

        public void AddUser(User newUser)
        {
            // Ensure user doesn't already exist
            if (_users.Any(u => u.Email == newUser.Email))
            {
                throw new Exception("User with this email already exists.");
            }

            // Assign a new ID
            newUser.Id = _users.Count + 1;

            // Add user to the list
            _users.Add(newUser);

            Console.WriteLine($"[DEBUG] New user added: {newUser.FullName} ({newUser.Email})");
        }

    }
}

