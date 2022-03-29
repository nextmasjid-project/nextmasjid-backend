using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
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
            services.AddSingleton(new MasterConnectionHolder());

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
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ScoreContext context,
            DbConnectionStringSupplier connectionStringSupplier,
            MasterConnectionHolder masterConnectionHolder)
        {
        //    EnsureDbCreated(context, connectionStringSupplier, masterConnectionHolder);

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

        private void EnsureDbCreated(ScoreContext context, DbConnectionStringSupplier dbConnectionStringSupplier,
            MasterConnectionHolder masterConnectionHolder)
        {
            context.Database.EnsureCreated();

            //delete this if and the db to recreate
            if (context.Scores.Any() == false)
            {
                SeedDb(dbConnectionStringSupplier);
            }

            LoadInMemory(dbConnectionStringSupplier, masterConnectionHolder);
        }

        private void LoadInMemory(DbConnectionStringSupplier dbConnectionStringSupplier, MasterConnectionHolder masterConnectionHolder)
        {
            int count;

            using (var contextCount = new ScoreContext(dbConnectionStringSupplier))
            {
                count = contextCount.Scores.Count();
            }

            var idx = 0;
            var page = 10000;

            SetMasterConnection(masterConnectionHolder);

            using (var contextCreate = new ScoreContext(true))
            {
                contextCreate.Database.EnsureCreated();
            }

            var contextRead = new ScoreContext(dbConnectionStringSupplier);

            while (idx * page < count)
            {
                var elems = contextRead.Scores.Skip(page * idx).Take(page).AsNoTracking().ToArray();

                using (var contextWrite = new ScoreContext(true))
                {
                    contextWrite.BulkInsert(elems);
                    contextWrite.SaveChanges();
                }

                ++idx;
            }
        }

        private static void SetMasterConnection(MasterConnectionHolder masterConnectionHolder)
        {
            const string connectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
            var masterConnection = new SqliteConnection(connectionString);
            masterConnection.Open();

            masterConnectionHolder.MasterConnection = masterConnection;
        }

        private void SeedDb(DbConnectionStringSupplier dbConnectionStringSupplier)
        {
            var path = Configuration["scoresPath"];
            var scores = DataReaderWriter.ReadScoresSegmented(path);

            var idx = 0;
            var page = 10000;

            while (idx * page < scores.Count)
            {
                using (var context = new ScoreContext(dbConnectionStringSupplier))
                {
                    var elems = scores.Skip(page * idx).Take(page).Select(s => new ScoreModelDb { Lat = s.Key.Item1, Lng = s.Key.Item2, Value = s.Value }).ToArray();

                    context.BulkInsert(elems);
                    context.SaveChanges();
                }

                ++idx;
            }
        }
    }
}
