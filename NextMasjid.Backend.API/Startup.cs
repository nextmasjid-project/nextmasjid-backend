using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NextMasjid.Backend.Core;
using NextMasjid.Backend.Core.Data;

namespace NextMasjid.Backend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();

            services.AddSwaggerGen();

            services.AddSingleton(new DbConnectionStringSupplier(Configuration["scoresDbPath"]));
            services.AddDbContext<ScoreContext>(options => options.UseSqlite());

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => {
                    options.AllowAnyOrigin();
                    options.AllowAnyMethod();
                    options.AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ScoreContext context, DbConnectionStringSupplier connectionStringSupplier)
        {
            EnsureDbCreated(context, connectionStringSupplier);

            app.UseSwagger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowOrigin");

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NextMasjid APIs v1.0.1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }



        private void EnsureDbCreated(ScoreContext context, DbConnectionStringSupplier dbConnectionStringSupplier)
        {
            context.Database.EnsureCreated();

            //delete this if and the db to recreate
            if (context.Scores.Any() == false)
            {
                SeedDb(dbConnectionStringSupplier);
            }
        }

        private void SeedDb(DbConnectionStringSupplier dbConnectionStringSupplier)
        {
            var path = Configuration["scoresPath"];
            var scores = DataReaderWriter.ReadScoresSegmented(path);

            var idx = 0;
            var page = 10000;

            while (idx * page < scores.Count)
            {
                var context = new ScoreContext(dbConnectionStringSupplier);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                var elems = scores.Skip(page * idx).Take(page).Select(s => new ScoreModelDb { Lat = s.Key.Item1, Lng = s.Key.Item2, Value = s.Value }).ToArray();

                context.Scores.AddRange(elems);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                context.ChangeTracker.AutoDetectChangesEnabled = true;

                context.Dispose();

                ++idx;
            }
        }
    }
}
