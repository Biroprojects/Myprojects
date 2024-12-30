using Kanban_board.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Kanban_board.Data;

public class Kanban_boardContext : IdentityDbContext<IdentityUser>
{
    public Kanban_boardContext(DbContextOptions<Kanban_boardContext> options)
        : base(options)
    {
    }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardUser> BoardUsers { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<KarbanTask> KanbanTasks { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<TaskLabel> TaskLabels { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<KarbanTask>()
                 .HasOne(kt => kt.TaskLabel)
                 .WithOne(tl => tl.Task)
                 .HasForeignKey<KarbanTask>(kt => kt.TaskLabelId); 

        builder.Entity<TaskLabel>()
            .HasOne(tl => tl.Label)
            .WithMany() 
            .HasForeignKey(tl => tl.LabelId); // A
    }
}
