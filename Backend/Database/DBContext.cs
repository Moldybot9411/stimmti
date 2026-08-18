using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class StimmtiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public StimmtiDbContext(DbContextOptions<StimmtiDbContext> options) : base(options) { }

    public DbSet<Folder> Folders { get; set; }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<QuestionTemplate> QuestionTemplates { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<AnonymousUser> AnonymousUsers { get; set; }
    public DbSet<AnonymousProfilePicture> AnonymousProfilePictures { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Folder>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.Folders)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Survey>()
            .HasOne(x => x.Folder)
            .WithMany(x => x.Surveys)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Survey>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.Surveys)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Session>()
            .HasOne(x => x.Survey)
            .WithMany(x => x.Sessions)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Session>()
            .HasOne(x => x.CurrentQuestion)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<QuestionTemplate>()
            .HasDiscriminator<QuestionTypeEnum>("QuestionTypeId")
            .HasValue<SingleChoiceQuestionTemplate>(QuestionTypeEnum.SingleChoice)
            .HasValue<MultipleChoiceQuestionTemplate>(QuestionTypeEnum.MultipleChoice)
            .HasValue<WordCloudQuestionTemplate>(QuestionTypeEnum.WordCloud)
            .HasValue<FreeTextQuestionTemplate>(QuestionTypeEnum.FreeText)
            .HasValue<NumberScaleQuestionTemplate>(QuestionTypeEnum.NumberScale);

        builder.Entity<QuestionTemplate>()
            .HasOne(x => x.Survey)
            .WithMany(x => x.QuestionTemplates)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChoiceQuestionTemplate>()
            .HasMany(x => x.AnswerOptions)
            .WithOne(x => x.QuestionTemplate);

        builder.Entity<AnonymousUser>()
            .HasOne(x => x.Session)
            .WithMany(x => x.AnonymousParticipants);

        builder.Entity<AnonymousUser>()
            .HasOne(x => x.ProfilePicture)
            .WithOne(x => x.AnonymousUser);

        builder.Entity<Answer>()
            .HasDiscriminator<QuestionTypeEnum>("QuestionTypeId")
            .HasValue<SingleChoiceAnswer>(QuestionTypeEnum.SingleChoice)
            .HasValue<MultipleChoiceAnswer>(QuestionTypeEnum.MultipleChoice)
            .HasValue<WordCloudAnswer>(QuestionTypeEnum.WordCloud)
            .HasValue<FreeTextAnswer>(QuestionTypeEnum.FreeText)
            .HasValue<NumberScaleAnswer>(QuestionTypeEnum.NumberScale);

        builder.Entity<Answer>()
            .HasOne(x => x.AnonymousUser)
            .WithMany(x => x.Answers);

        builder.Entity<Answer>()
            .HasOne(x => x.Question)
            .WithMany(x => x.Answers);

        builder.Entity<ChoiceAnswer>()
            .HasOne(x => x.AnswerOption)
            .WithMany(x => x.Answers);
    }
}