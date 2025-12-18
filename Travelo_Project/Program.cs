using DomainLayer.Models.Identity;
using DomainLayer.Models.User;
using DomainLayer.RepositoryInterface;
using DomainLayer.RepositoryInterface.Flights;
using DomainLayer.RepositoryInterface.Hotel___Accommodation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Persistance;
using Persistance.Configuration.Tour;
using Persistance.Identity;
using Persistance.RepositoryImplementation;
using Persistance.RepositoryImplementation.Flights;
using Persistance.RepositoryImplementation.Hotel___Accommodation;
using Persistance.RepositoryImplementation.Hotel___Accomodation;
using presentation.Controllers;
using ServiceAbstraction;
using ServiceAbstraction.flight;
using ServiceAbstraction.Hotel___Accommodation;
using ServiceImplementation;
using ServiceImplementation.flight;
using ServiceImplementation.Flight;
using ServiceImplementation.Hotel___Accommodation;
using ServiceImplementation.MappingProfile;
using ServiceImplementation.MappingProfile.Hotel___Accommodation;
using Shared;
using System.Text;

namespace Travelo_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
           .AddApplicationPart(typeof(AuthController).Assembly)
           .AddApplicationPart(typeof(PaymentController).Assembly)
           .AddApplicationPart(typeof(ReviewController).Assembly);
            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("BaseConnection"));



            });



            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

    



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtOptions:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtOptions:Audience"],

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Key"])
                    )
                };
            });

         



            builder.Services.AddScoped<IAirlineService, AirlineService>();
            builder.Services.AddScoped<IAirlineRepository, AirLineRepository>();
            builder.Services.AddAutoMapper(typeof(AirlineProfile).Assembly);
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IAirportRepository, AirportRepository>();
            builder.Services.AddScoped<IAirportService, AirportService>();
            builder.Services.AddScoped<IFlightService, FlightService>();

            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<IFlightOfferRepository, FlightOfferRepository>();
            builder.Services.AddScoped<IFlightPriceRepository, FlightPriceRepository>();
            builder.Services.AddScoped<IFlightPriceRepository, FlightPriceRepository>();
            builder.Services.AddScoped<IFlightPriceService, FlightPriceService>();
            builder.Services.AddScoped<IFlightOfferService, FlightOfferService>();



            builder.Services.AddAutoMapper(typeof(AuthProfile));
            builder.Services.AddAutoMapper(typeof(BookingProfile));
            builder.Services.AddAutoMapper(typeof(ReviewProfile));



            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            builder.Services.AddScoped(typeof(IReviewServices), typeof(ReviewServices));
            builder.Services.AddScoped(typeof(ITransactionService), typeof(TransactionService));

            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();
            builder.Services.AddAutoMapper(typeof(AmenityProfile));


            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddAutoMapper(typeof(RoomProfile));

            builder.Services.AddScoped<IHotelAmenityRepository, HotelAmenityRepository>();
            builder.Services.AddScoped<IHotelAmenityService, HotelAmenityService>();
            builder.Services.AddAutoMapper(typeof(HotelAmenityProfile));

            builder.Services.AddScoped<IHotelRepository, HotelRepository>();
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddAutoMapper(typeof(HotelProfile));
            builder.Services.AddAutoMapper(typeof(HotelDetailsProfile));

            builder.Services.AddScoped<IPriceAndAvailabilityRepository, PriceAndAvailabilityRepository>();
            builder.Services.AddScoped<IPriceAndAvailbilityService, PriceAndAvailbilityService>();
            builder.Services.AddAutoMapper(typeof(PriceAndAvailabilityProfile));

            //builder.Services.AddScoped<DestinationConfiguration, >();
            //builder.Services.AddScoped<TourConfiguration, >();
            //builder.Services.AddScoped<TourDateConfiguration, >();
            //builder.Services.AddScoped<TourDestinationConfiguration, >();
            //builder.Services.AddScoped<TourInclusionConfiguration, >();
            //builder.Services.AddScoped<TourItineraryConfiguration, >();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();

                    await IdentityDataSeed.Initialize(roleManager, userManager, dbContext);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseHttpsRedirection();


                app.MapControllers();

                app.Run();
            }
        }
    }
}
