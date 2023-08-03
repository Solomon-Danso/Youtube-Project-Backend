using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using YouTube_Backend.Models;

namespace YouTube_Backend.Data
{
    public class DataContext:DbContext
    {
                //Empty constructor
public DataContext(): base(){
}


protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
base.OnConfiguring(optionsBuilder); optionsBuilder.UseSqlServer("Server=localhost,1433;Database=YouTubeProject;User=sa;Password=HydotTech;TrustServerCertificate=true;");
}



public DbSet<VideoModel> VideoModels { get; set; }
public DbSet<UserAccount> UserAccounts { get; set; }
public DbSet<AdminAccount> AdminAccounts { get; set; }
public DbSet<Like> Likes { get; set; }
public DbSet<DisLike> DisLikes { get; set; }
public DbSet<VideoComment> VideoComments { get; set; }
public DbSet<VideoCommentsReply> VideoCommentsReplys { get; set; }
public DbSet<UploadVideoNotification> UploadVideoNotifications { get; set; }
public DbSet<Message> Messages { get; set; }

    }
}