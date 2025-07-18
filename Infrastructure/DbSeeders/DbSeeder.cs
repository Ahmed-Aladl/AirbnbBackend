using Domain.Models;
using Domain.Enums.Booking;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AirbnbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            //if (await context.Users.AnyAsync()||
            //    await context.Properties.AnyAsync()||
            //    await context.PropertyAmenities.AnyAsync() || 
            //    await context.propertyTypes.AnyAsync()||
            //    await context.Wishlist.AnyAsync()||
            //    await context.Bookings.AnyAsync()


            //    )
            //{
            //    return; // Database has been seeded
            //}
            if (!await context.Roles.AnyAsync())
                // Create roles
                await CreateRoles(roleManager);

            // Create users
            List<User> users = new();
            if (!await context.Users.AnyAsync())
                users = await CreateUsers(userManager);

            List<PropertyType> propertyTypes = new();
            if (!await context.propertyTypes.AnyAsync())
                // Create property types
                propertyTypes = await CreatePropertyTypes(context);

            List<Amenity> amenities = new();
            if (!await context.Amenities.AnyAsync())
                // Create amenities
                amenities = await CreateAmenities(context);


            List<Property> properties = new();
            if (!await context.Properties.AnyAsync())
                // Create properties
                properties = await CreateProperties(context, users, propertyTypes);


            if (!await context.PropertyAmenities.AnyAsync())
                // Create property amenities
                await CreatePropertyAmenities(context, properties, amenities);

            if (!await context.PropertyImages.AnyAsync())
                // Create property images
                await CreatePropertyImages(context, properties);

            if (!await context.calendarAvailabilities.AnyAsync())
                // Create calendar availabilities
                await CreateCalendarAvailabilities(context, properties);

            // Create bookings
            List<Booking> bookings = new();
            if (!await context.Bookings.AnyAsync())
                bookings = await CreateBookings(context, users, properties);

            // Create payments
            if (!await context.Payments.AnyAsync())
                await CreatePayments(context, bookings);

            // Create reviews
            if (!await context.Reviews.AnyAsync())
                await CreateReviews(context, users, properties, bookings);

            // Create wishlists
            if (!await context.Wishlist.AnyAsync())
                await CreateWishlists(context, users, properties);

            // Create notifications
            if (!await context.Notifications.AnyAsync())
                await CreateNotifications(context, users);

            // Create messages
            if (!await context.Messages.AnyAsync())
                await CreateMessages(context, users, properties);


            await context.SaveChangesAsync();
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Host", "Guest" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task<List<User>> CreateUsers(UserManager<User> userManager)
        {
            var users = new List<User>
            {
                new User
                {
                    UserName = "admin@airbnb.com",
                    Email = "admin@airbnb.com",
                    FirstName = "Admin",
                    LastName = "User",
                    CreateAt = DateTime.Now.AddDays(-365),
                    UpdatedAt = DateTime.Now,
                    ProfilePictureURL = "https://example.com/admin-profile.jpg",
                    Bio = "System Administrator",
                    Country = "USA",
                    BirthDate = new DateOnly(1990, 1, 1),
                    EmailConfirmed = true
                },
                new User
                {
                    UserName = "john.host@email.com",
                    Email = "john.host@email.com",
                    FirstName = "John",
                    LastName = "Smith",
                    CreateAt = DateTime.Now.AddDays(-200),
                    UpdatedAt = DateTime.Now,
                    ProfilePictureURL = "https://example.com/john-profile.jpg",
                    Bio = "Experienced host with 5 years of hosting experience",
                    Country = "USA",
                    BirthDate = new DateOnly(1985, 3, 15),
                    EmailConfirmed = true
                },
                new User
                {
                    UserName = "sarah.host@email.com",
                    Email = "sarah.host@email.com",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    CreateAt = DateTime.Now.AddDays(-180),
                    UpdatedAt = DateTime.Now,
                    ProfilePictureURL = "https://example.com/sarah-profile.jpg",
                    Bio = "Luxury property host specializing in beachfront rentals",
                    Country = "France",
                    BirthDate = new DateOnly(1988, 7, 22),
                    EmailConfirmed = true
                },
                new User
                {
                    UserName = "mike.guest@email.com",
                    Email = "mike.guest@email.com",
                    FirstName = "Mike",
                    LastName = "Davis",
                    CreateAt = DateTime.Now.AddDays(-120),
                    UpdatedAt = DateTime.Now,
                    ProfilePictureURL = "https://example.com/mike-profile.jpg",
                    Bio = "Travel enthusiast and digital nomad",
                    Country = "Canada",
                    BirthDate = new DateOnly(1992, 11, 8),
                    EmailConfirmed = true
                },
                new User
                {
                    UserName = "emma.guest@email.com",
                    Email = "emma.guest@email.com",
                    FirstName = "Emma",
                    LastName = "Wilson",
                    CreateAt = DateTime.Now.AddDays(-90),
                    UpdatedAt = DateTime.Now,
                    ProfilePictureURL = "https://example.com/emma-profile.jpg",
                    Bio = "Family vacation planner and food lover",
                    Country = "UK",
                    BirthDate = new DateOnly(1995, 4, 12),
                    EmailConfirmed = true
                }
            };

            var createdUsers = new List<User>();
            string[] passwords = { "Admin@123", "Host@123", "Host@123", "Guest@123", "Guest@123" };
            string[] roles = { "Admin", "Host", "Host", "Guest", "Guest" };

            for (int i = 0; i < users.Count; i++)
            {
                var result = await userManager.CreateAsync(users[i], passwords[i]);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(users[i], roles[i]);
                    createdUsers.Add(users[i]);
                }
            }

            return createdUsers;
        }

        private static async Task<List<PropertyType>> CreatePropertyTypes(AirbnbContext context)
        {
            var propertyTypes = new List<PropertyType>
            {
                new PropertyType { Name = "Apartment" },
                new PropertyType { Name = "House" },
                new PropertyType { Name = "Villa" },
                new PropertyType { Name = "Condo" },
                new PropertyType { Name = "Cabin" },
                new PropertyType { Name = "Loft" },
                new PropertyType { Name = "Studio" }
            };

            context.propertyTypes.AddRange(propertyTypes);
            await context.SaveChangesAsync();
            return propertyTypes;
        }

        private static async Task<List<Amenity>> CreateAmenities(AirbnbContext context)
        {
            var amenities = new List<Amenity>
            {
                new Amenity { AmenityName = "WiFi", IconURL = "https://example.com/icons/wifi.png" },
                new Amenity { AmenityName = "Kitchen", IconURL = "https://example.com/icons/kitchen.png" },
                new Amenity { AmenityName = "Air Conditioning", IconURL = "https://example.com/icons/ac.png" },
                new Amenity { AmenityName = "Parking", IconURL = "https://example.com/icons/parking.png" },
                new Amenity { AmenityName = "Pool", IconURL = "https://example.com/icons/pool.png" },
                new Amenity { AmenityName = "Gym", IconURL = "https://example.com/icons/gym.png" },
                new Amenity { AmenityName = "Pet Friendly", IconURL = "https://example.com/icons/pet.png" },
                new Amenity { AmenityName = "Washing Machine", IconURL = "https://example.com/icons/washing.png" },
                new Amenity { AmenityName = "TV", IconURL = "https://example.com/icons/tv.png" },
                new Amenity { AmenityName = "Balcony", IconURL = "https://example.com/icons/balcony.png" }
            };

            context.Amenities.AddRange(amenities);
            await context.SaveChangesAsync();
            return amenities;
        }

        private static async Task<List<Property>> CreateProperties(AirbnbContext context, List<User> users, List<PropertyType> propertyTypes)
        {
            var hosts = users.Where(u => u.Email.Contains("host")).ToList();
            var properties = new List<Property>
            {
                new Property
                {
                    Title = "Luxury Beachfront Villa",
                    Description = "Stunning villa with panoramic ocean views and private beach access",
                    City = "Miami",
                    Country = "USA",
                    State = "Florida",
                    Latitude = 25.7617m,
                    Longitude = -80.1918m,
                    PricePerNight = 450.00m,
                    MaxGuests = 8,
                    Bedrooms = 4,
                    Beds = 6,
                    Bathrooms = 3,
                    AverageRating = 4.8f,
                    ReviewCount = 24,
                    IsActive = true,
                    PropertyTypeId = propertyTypes.First(pt => pt.Name == "Villa").Id,
                    HostId = hosts[0].Id
                },
                new Property
                {
                    Title = "Cozy Downtown Apartment",
                    Description = "Modern apartment in the heart of the city with great amenities",
                    City = "New York",
                    Country = "USA",
                    State = "New York",
                    Latitude = 40.7128m,
                    Longitude = -74.0060m,
                    PricePerNight = 180.00m,
                    MaxGuests = 4,
                    Bedrooms = 2,
                    Beds = 3,
                    Bathrooms = 2,
                    AverageRating = 4.5f,
                    ReviewCount = 18,
                    IsActive = true,
                    PropertyTypeId = propertyTypes.First(pt => pt.Name == "Apartment").Id,
                    HostId = hosts[0].Id
                },
                new Property
                {
                    Title = "Charming Parisian Loft",
                    Description = "Authentic Parisian experience in a beautiful historic building",
                    City = "Paris",
                    Country = "France",
                    State = "Île-de-France",
                    Latitude = 48.8566m,
                    Longitude = 2.3522m,
                    PricePerNight = 220.00m,
                    MaxGuests = 2,
                    Bedrooms = 1,
                    Beds = 1,
                    Bathrooms = 1,
                    AverageRating = 4.9f,
                    ReviewCount = 31,
                    IsActive = true,
                    PropertyTypeId = propertyTypes.First(pt => pt.Name == "Loft").Id,
                    HostId = hosts[1].Id
                },
                new Property
                {
                    Title = "Mountain Cabin Retreat",
                    Description = "Peaceful cabin surrounded by nature, perfect for a getaway",
                    City = "Aspen",
                    Country = "USA",
                    State = "Colorado",
                    Latitude = 39.1911m,
                    Longitude = -106.8175m,
                    PricePerNight = 320.00m,
                    MaxGuests = 6,
                    Bedrooms = 3,
                    Beds = 4,
                    Bathrooms = 2,
                    AverageRating = 4.7f,
                    ReviewCount = 15,
                    IsActive = true,
                    PropertyTypeId = propertyTypes.First(pt => pt.Name == "Cabin").Id,
                    HostId = hosts[1].Id
                },
                new Property
                {
                    Title = "Modern Studio in City Center",
                    Description = "Stylish studio perfect for business travelers",
                    City = "London",
                    Country = "UK",
                    State = "England",
                    Latitude = 51.5074m,
                    Longitude = -0.1278m,
                    PricePerNight = 120.00m,
                    MaxGuests = 2,
                    Bedrooms = 1,
                    Beds = 1,
                    Bathrooms = 1,
                    AverageRating = 4.3f,
                    ReviewCount = 12,
                    IsActive = true,
                    PropertyTypeId = propertyTypes.First(pt => pt.Name == "Studio").Id,
                    HostId = hosts[0].Id
                }
            };

            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();
            return properties;
        }

        private static async Task CreatePropertyAmenities(AirbnbContext context, List<Property> properties, List<Amenity> amenities)
        {
            var propertyAmenities = new List<PropertyAmenity>();

            foreach (var property in properties)
            {
                // Add random amenities to each property
                var randomAmenities = amenities.OrderBy(x => Guid.NewGuid()).Take(Random.Shared.Next(3, 8)).ToList();

                foreach (var amenity in randomAmenities)
                {
                    propertyAmenities.Add(new PropertyAmenity
                    {
                        PropertyId = property.Id,
                        AmenityId = amenity.Id
                    });
                }
            }

            context.PropertyAmenities.AddRange(propertyAmenities);
            await context.SaveChangesAsync();
        }

        private static async Task CreatePropertyImages(AirbnbContext context, List<Property> properties)
        {
            var propertyImages = new List<PropertyImage>();

            foreach (var property in properties)
            {
                // Add 3-5 images per property
                for (int i = 1; i <= Random.Shared.Next(3, 6); i++)
                {
                    propertyImages.Add(new PropertyImage
                    {
                        PropertyId = property.Id,
                        ImageUrl = $"https://example.com/property-{property.Id}-image-{i}.jpg",
                        IsCover = i == 1, // First image is cover
                        GroupName = $"Property {property.Id} Images"
                    });
                }
            }

            context.PropertyImages.AddRange(propertyImages);
            await context.SaveChangesAsync();
        }

        private static async Task CreateCalendarAvailabilities(AirbnbContext context, List<Property> properties)
        {
            var availabilities = new List<CalendarAvailability>();

            foreach (var property in properties)
            {
                // Create availability for next 6 months
                for (int days = 0; days < 180; days++)
                {
                    var date = DateTime.Today.AddDays(days);
                    var isAvailable = Random.Shared.Next(0, 10) > 2; // 80% availability
                    var priceVariation = (decimal)(Random.Shared.NextDouble() * 0.3 - 0.15); // ±15% price variation
                    var price = property.PricePerNight * (1 + priceVariation);

                    availabilities.Add(new CalendarAvailability
                    {
                        PropertyId = property.Id,
                        Date = date,
                        IsAvailable = isAvailable,
                        Price = Math.Round(price, 2)
                    });
                }
            }

            context.calendarAvailabilities.AddRange(availabilities);
            await context.SaveChangesAsync();
        }

        private static async Task<List<Booking>> CreateBookings(AirbnbContext context, List<User> users, List<Property> properties)
        {
            var guests = users.Where(u => u.Email.Contains("guest")).ToList();
            var bookings = new List<Booking>();

            foreach (var guest in guests)
            {
                // Create 2-3 bookings per guest
                for (int i = 0; i < Random.Shared.Next(2, 4); i++)
                {
                    var property = properties[Random.Shared.Next(properties.Count)];
                    var checkIn = DateTime.Today.AddDays(Random.Shared.Next(-30, 60));
                    var nights = Random.Shared.Next(2, 8);
                    var checkOut = checkIn.AddDays(nights);

                    bookings.Add(new Booking
                    {
                        CheckInDate = checkIn,
                        CheckOutDate = checkOut,
                        NumberOfGuests = Random.Shared.Next(1, property.MaxGuests + 1),
                        TotalPrice = property.PricePerNight * nights,
                        BookingStatus = (BookingStatus)Random.Shared.Next(0, 4),
                        PropertyId = property.Id,
                        UserId = guest.Id
                    });
                }
            }

            context.Bookings.AddRange(bookings);
            await context.SaveChangesAsync();
            return bookings;
        }

        private static async Task CreatePayments(AirbnbContext context, List<Booking> bookings)
        {
            var payments = new List<Payment>();

            foreach (var booking in bookings.Where(b => b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.Completed))
            {
                payments.Add(new Payment
                {
                    BookingId = booking.Id,
                    Amount = (int)(booking.TotalPrice * 100), // Convert to cents
                    PaymentDate = booking.CheckInDate.AddDays(-1),
                    Status = "Completed"
                });
            }

            context.Payments.AddRange(payments);
            await context.SaveChangesAsync();
        }

        private static async Task CreateReviews(AirbnbContext context, List<User> users, List<Property> properties, List<Booking> bookings)
        {
            var reviews = new List<Review>();
            var completedBookings = bookings.Where(b => b.BookingStatus == BookingStatus.Completed).ToList();

            foreach (var booking in completedBookings.Take(10)) // Create reviews for first 10 completed bookings
            {
                var rating = Random.Shared.Next(3, 6); // Ratings between 3-5
                var comments = new[]
                {
                    "Amazing place! Highly recommend.",
                    "Great location and very clean.",
                    "Perfect for our vacation needs.",
                    "Host was very responsive and helpful.",
                    "Beautiful property with stunning views.",
                    "Would definitely stay again!"
                };

                reviews.Add(new Review
                {
                    UserId = booking.UserId,
                    PropertyId = booking.PropertyId,
                    BookingId = booking.Id,
                    Rating = rating,
                    Comment = comments[Random.Shared.Next(comments.Length)]
                });
            }

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }

        private static async Task CreateWishlists(AirbnbContext context, List<User> users, List<Property> properties)
        {
            var guests = users.Where(u => u.Email.Contains("guest")).ToList();
            var wishlists = new List<Wishlist>();

            foreach (var guest in guests)
            {
                // Add 2-4 properties to each guest's wishlist
                var randomProperties = properties.OrderBy(x => Guid.NewGuid()).Take(Random.Shared.Next(2, 5)).ToList();

                foreach (var property in randomProperties)
                {
                    var wishlistNames = new[]
                    {
                        "Dream Vacation Spots",
                        "Weekend Getaways",
                        "Family Trips",
                        "Romantic Escapes",
                        "Business Travel",
                        "Adventure Destinations"
                    };

                    var wishlistNotes = new[]
                    {
                        "Perfect for next vacation!",
                        "Great location and amenities",
                        "Bookmarked for future reference",
                        "Looks amazing in photos",
                        "Recommended by friends",
                        "Good value for money"
                    };

                    wishlists.Add(new Wishlist
                    {
                        UserId = guest.Id,
                        PropertyId = property.Id,
                        Name = wishlistNames[Random.Shared.Next(wishlistNames.Length)],
                        Notes = wishlistNotes[Random.Shared.Next(wishlistNotes.Length)],
                        CreatedAt = DateTime.Now.AddDays(-Random.Shared.Next(1, 30))
                    });
                }
            }

            context.Wishlist.AddRange(wishlists);
            await context.SaveChangesAsync();
        }

        private static async Task CreateNotifications(AirbnbContext context, List<User> users)
        {
            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                // Create 3-5 notifications per user
                for (int i = 0; i < Random.Shared.Next(3, 6); i++)
                {
                    var messages = new[]
                    {
                        "Welcome to our platform!",
                        "Your booking has been confirmed.",
                        "Don't forget to leave a review.",
                        "New property matches your preferences.",
                        "Payment received successfully.",
                        "Your reservation is coming up soon."
                    };

                    notifications.Add(new Notification
                    {
                        UserId = user.Id,
                        Message = messages[Random.Shared.Next(messages.Length)],
                        isRead = Random.Shared.Next(0, 2) == 1,
                        CreatedAt = DateTime.Now.AddDays(-Random.Shared.Next(1, 30))
                    });
                }
            }

            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();
        }

        private static async Task CreateMessages(AirbnbContext context, List<User> users, List<Property> properties)
        {
            var messages = new List<Message>();
            var hosts = users.Where(u => u.Email.Contains("host")).ToList();
            var guests = users.Where(u => u.Email.Contains("guest")).ToList();

            // Create conversations between hosts and guests
            foreach (var host in hosts)
            {
                foreach (var guest in guests.Take(2)) // Each host talks to 2 guests
                {
                    var property = properties.FirstOrDefault(p => p.HostId == host.Id);
                    if (property != null)
                    {
                        // Create a conversation thread
                        var conversationMessages = new[]
                        {
                            new Message
                            {
                                SenderId = guest.Id,
                                ReceiverId = host.Id,
                                PropertyId = property.Id,
                                MesageText = "Hi, I'm interested in booking your property. Is it available for next weekend?",
                                SentAt = DateTime.Now.AddHours(-24),
                                Isread = true
                            },
                            new Message
                            {
                                SenderId = host.Id,
                                ReceiverId = guest.Id,
                                PropertyId = property.Id,
                                MesageText = "Hello! Yes, it's available. I'd be happy to host you. Do you have any questions about the property?",
                                SentAt = DateTime.Now.AddHours(-23),
                                Isread = true
                            },
                            new Message
                            {
                                SenderId = guest.Id,
                                ReceiverId = host.Id,
                                PropertyId = property.Id,
                                MesageText = "Great! What's the check-in process like?",
                                SentAt = DateTime.Now.AddHours(-22),
                                Isread = false
                            }
                        };

                        messages.AddRange(conversationMessages);
                    }
                }
            }

            context.Messages.AddRange(messages);
            await context.SaveChangesAsync();
        }
    }
}