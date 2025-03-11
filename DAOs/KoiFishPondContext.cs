using System;
using System.Collections.Generic;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DAOs;

public partial class KoiFishPondContext : DbContext
{
    public KoiFishPondContext()
    {
    }

    public KoiFishPondContext(DbContextOptions<KoiFishPondContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<BookingOffline> BookingOfflines { get; set; }

    public virtual DbSet<BookingOnline> BookingOnlines { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<CompatibilityPoint> CompatibilityPoints { get; set; }

    public virtual DbSet<ConsultationPackage> ConsultationPackages { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ElementPoint> ElementPoints { get; set; }

    public virtual DbSet<EnrollAnswer> EnrollAnswers { get; set; }

    public virtual DbSet<EnrollCert> EnrollCerts { get; set; }

    public virtual DbSet<EnrollChapter> EnrollChapters { get; set; }

    public virtual DbSet<EnrollQuiz> EnrollQuizzes { get; set; }

    public virtual DbSet<FengShuiDocument> FengShuiDocuments { get; set; }

    public virtual DbSet<KoiPond> KoiPonds { get; set; }

    public virtual DbSet<KoiVariety> KoiVarieties { get; set; }

    public virtual DbSet<MansionsPoint> MansionsPoints { get; set; }

    public virtual DbSet<Master> Masters { get; set; }

    public virtual DbSet<MasterSchedule> MasterSchedules { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<RegisterAttend> RegisterAttends { get; set; }

    public virtual DbSet<RegisterCourse> RegisterCourses { get; set; }

    public virtual DbSet<Shape> Shapes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<VarietyColor> VarietyColors { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    public virtual DbSet<WorkShop> WorkShops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-59R56V3A\\SQLEXPRESS;Initial Catalog=KoiFishPond;Persist Security Info=True;User ID=sa;Password=123456;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6721DBE77");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answer__D482500449C06126");

            entity.ToTable("Answer");

            entity.Property(e => e.AnswerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreateAt).HasColumnName("Create_At");
            entity.Property(e => e.IsCorrect).HasColumnName("Is_Correct");
            entity.Property(e => e.OptionText).HasColumnName("Option_Text");
            entity.Property(e => e.OptionType)
                .HasMaxLength(50)
                .HasColumnName("Option_Type");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Answer__Question__7A672E12");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__442C64BEA1993E4F");

            entity.ToTable("Attachment");

            entity.Property(e => e.AttachmentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AttachmentName).HasMaxLength(100);
            entity.Property(e => e.DocNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<BookingOffline>(entity =>
        {
            entity.HasKey(e => e.BookingOfflineId).HasName("PK__BookingO__37BA370314EE5ECC");

            entity.ToTable("BookingOffline");

            entity.Property(e => e.BookingOfflineId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ConsultationPackageId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ContractId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DocumentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.MasterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MasterScheduleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RecordId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.ConsultationPackage).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.ConsultationPackageId)
                .HasConstraintName("FK__BookingOf__Consu__7B5B524B");

            entity.HasOne(d => d.Contract).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__BookingOf__Contr__7C4F7684");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__BookingOf__Custo__7D439ABD");

            entity.HasOne(d => d.Document).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.DocumentId)
                .HasConstraintName("FK__BookingOf__Docum__7E37BEF6");

            entity.HasOne(d => d.Master).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.MasterId)
                .HasConstraintName("FK__BookingOf__Maste__7F2BE32F");

            entity.HasOne(d => d.MasterSchedule).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.MasterScheduleId)
                .HasConstraintName("FK_BookingOffline_MasterSchedule");

            entity.HasOne(d => d.Record).WithMany(p => p.BookingOfflines)
                .HasForeignKey(d => d.RecordId)
                .HasConstraintName("FK__BookingOf__Recor__00200768");
        });

        modelBuilder.Entity<BookingOnline>(entity =>
        {
            entity.HasKey(e => e.BookingOnlineId).HasName("PK__BookingO__D0E9AFED39860FB3");

            entity.ToTable("BookingOnline");

            entity.Property(e => e.BookingOnlineId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.LinkMeet).HasMaxLength(255);
            entity.Property(e => e.MasterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MasterNote).HasMaxLength(255);
            entity.Property(e => e.MasterScheduleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingOnlines)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingOnline_Customer");

            entity.HasOne(d => d.Master).WithMany(p => p.BookingOnlines)
                .HasForeignKey(d => d.MasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingOnline_Master");

            entity.HasOne(d => d.MasterSchedule).WithMany(p => p.BookingOnlines)
                .HasForeignKey(d => d.MasterScheduleId)
                .HasConstraintName("FK_BookingOnline_MasterSchedule");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__BBF8A7C15AAC254D");

            entity.ToTable("Certificate");

            entity.Property(e => e.CertificateId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CertificateImage).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__Chapter__0893A36A3F641DA0");

            entity.ToTable("Chapter");

            entity.Property(e => e.ChapterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CourseId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Video).HasMaxLength(255);

            entity.HasOne(d => d.Course).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Chapter__CourseI__04E4BC85");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__Color__8DA7674D986C53F4");

            entity.ToTable("Color");

            entity.Property(e => e.ColorId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ColorCode)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.ColorName).HasMaxLength(50);
            entity.Property(e => e.Element).HasMaxLength(20);
        });

        modelBuilder.Entity<CompatibilityPoint>(entity =>
        {
            entity.HasKey(e => e.CompatibilityType).HasName("PK__Compatib__490B3B31068AEC2B");

            entity.ToTable("CompatibilityPoint");

            entity.Property(e => e.CompatibilityType)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConsultationPackage>(entity =>
        {
            entity.HasKey(e => e.ConsultationPackageId).HasName("PK__Consulta__3A64C556AA836FE2");

            entity.ToTable("ConsultationPackage");

            entity.Property(e => e.ConsultationPackageId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.PackagePrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34698B1C1EBA");

            entity.ToTable("Contract");

            entity.Property(e => e.ContractId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ContractName).HasMaxLength(100);
            entity.Property(e => e.DocNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A74CA02688");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CertificateId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CourseCategory).HasMaxLength(50);
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.QuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Certificate).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CertificateId)
                .HasConstraintName("FK__Course__Certific__05D8E0BE");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Courses)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK__Course__QuizId__06CD04F7");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D825F1632F");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AccountId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Element).HasMaxLength(20);
            entity.Property(e => e.LifePalace).HasMaxLength(20);
            entity.Property(e => e.MembershipId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Account).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Customer__Accoun__07C12930");

            entity.HasOne(d => d.Membership).WithMany(p => p.Customers)
                .HasForeignKey(d => d.MembershipId)
                .HasConstraintName("FK__Customer__Member__08B54D69");
        });

        modelBuilder.Entity<ElementPoint>(entity =>
        {
            entity.HasKey(e => e.ElementType).HasName("PK__ElementP__D7C667979B4D7D2D");

            entity.ToTable("ElementPoint");

            entity.Property(e => e.ElementType)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EnrollAnswer>(entity =>
        {
            entity.HasKey(e => e.EnrollAnswerId).HasName("PK__EnrollAn__913A38558EFFFAF1");

            entity.ToTable("EnrollAnswer");

            entity.Property(e => e.EnrollAnswerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AnswerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Answer).WithMany(p => p.EnrollAnswers)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("FK__EnrollAns__Answe__09A971A2");
        });

        modelBuilder.Entity<EnrollCert>(entity =>
        {
            entity.HasKey(e => e.EnrollCertId).HasName("PK__EnrollCe__15C91EE922641C71");

            entity.ToTable("EnrollCert");

            entity.Property(e => e.EnrollCertId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CertificateId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Certificate).WithMany(p => p.EnrollCerts)
                .HasForeignKey(d => d.CertificateId)
                .HasConstraintName("FK__EnrollCer__Certi__0A9D95DB");

            entity.HasOne(d => d.Customer).WithMany(p => p.EnrollCerts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__EnrollCer__Custo__0B91BA14");
        });

        modelBuilder.Entity<EnrollChapter>(entity =>
        {
            entity.HasKey(e => e.EnrollChapterId).HasName("PK__EnrollCh__035269FBB41A8D80");

            entity.ToTable("EnrollChapter");

            entity.Property(e => e.EnrollChapterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ChapterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Chapter).WithMany(p => p.EnrollChapters)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK__EnrollCha__Chapt__0C85DE4D");
        });

        modelBuilder.Entity<EnrollQuiz>(entity =>
        {
            entity.HasKey(e => e.EnrollQuizId).HasName("PK__EnrollQu__B842C3666741BE2B");

            entity.ToTable("EnrollQuiz");

            entity.Property(e => e.EnrollQuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EnrollAnswerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ParticipantId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Point).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.EnrollAnswer).WithMany(p => p.EnrollQuizzes)
                .HasForeignKey(d => d.EnrollAnswerId)
                .HasConstraintName("FK__EnrollQui__Enrol__0D7A0286");

            entity.HasOne(d => d.Participant).WithMany(p => p.EnrollQuizzes)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("FK__EnrollQui__Parti__0E6E26BF");

            entity.HasOne(d => d.Quiz).WithMany(p => p.EnrollQuizzes)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK__EnrollQui__QuizI__0F624AF8");
        });

        modelBuilder.Entity<FengShuiDocument>(entity =>
        {
            entity.HasKey(e => e.FengShuiDocumentId).HasName("PK__FengShui__AFCA4193A4BA2C55");

            entity.ToTable("FengShuiDocument");

            entity.Property(e => e.FengShuiDocumentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DocNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DocumentName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Version)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KoiPond>(entity =>
        {
            entity.HasKey(e => e.KoiPondId).HasName("PK__KoiPond__8DAE659AFF52FD95");

            entity.ToTable("KoiPond");

            entity.Property(e => e.KoiPondId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Direction).HasMaxLength(20);
            entity.Property(e => e.PondName).HasMaxLength(100);
            entity.Property(e => e.ShapeId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Shape).WithMany(p => p.KoiPonds)
                .HasForeignKey(d => d.ShapeId)
                .HasConstraintName("FK__KoiPond__ShapeId__10566F31");
        });

        modelBuilder.Entity<KoiVariety>(entity =>
        {
            entity.HasKey(e => e.KoiVarietyId).HasName("PK__KoiVarie__94DDFCB27D48FE7E");

            entity.ToTable("KoiVariety");

            entity.Property(e => e.KoiVarietyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("KoiVarietyID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.VarietyName).HasMaxLength(50);
        });

        modelBuilder.Entity<MansionsPoint>(entity =>
        {
            entity.HasKey(e => e.MansionsType).HasName("PK__Mansions__1D5CE3F70290151E");

            entity.ToTable("MansionsPoint");

            entity.Property(e => e.MansionsType)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Master>(entity =>
        {
            entity.HasKey(e => e.MasterId).HasName("PK__Master__F6B7822453CF10A2");

            entity.ToTable("Master");

            entity.Property(e => e.MasterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AccountId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Experience).HasMaxLength(50);
            entity.Property(e => e.Expertise).HasMaxLength(255);
            entity.Property(e => e.MasterName).HasMaxLength(100);
            entity.Property(e => e.ServiceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Account).WithMany(p => p.Masters)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Master__AccountI__114A936A");
        });

        modelBuilder.Entity<MasterSchedule>(entity =>
        {
            entity.HasKey(e => e.MasterScheduleId).HasName("PK__MasterSc__F5937CE593D01D25");

            entity.ToTable("MasterSchedule");

            entity.Property(e => e.MasterScheduleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MasterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Master).WithMany(p => p.MasterSchedules)
                .HasForeignKey(d => d.MasterId)
                .HasConstraintName("FK__MasterSch__Maste__123EB7A3");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__Membersh__92A78679A693FEDF");

            entity.ToTable("Membership");

            entity.Property(e => e.MembershipId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MembershipName).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC0746936B5D");

            entity.ToTable("Order");

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Note)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PaymentReference)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ServiceId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Order_Customer");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACC86F3805");

            entity.ToTable("Question");

            entity.Property(e => e.QuestionId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreateAt).HasColumnName("Create_At");
            entity.Property(e => e.Point).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QuestionText).HasColumnName("Question_Text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .HasColumnName("Question_Type");
            entity.Property(e => e.QuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK__Question__QuizId__1332DBDC");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.QuizId).HasName("PK__Quiz__8B42AE8E1A62B57C");

            entity.ToTable("Quiz");

            entity.Property(e => e.QuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CompletedAt).HasColumnName("Completed_At");
            entity.Property(e => e.CourseId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreateAt).HasColumnName("Create_At");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Create_By");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Quiz__CourseId__14270015");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK__Quiz__Create_By__151B244E");
        });

        modelBuilder.Entity<RegisterAttend>(entity =>
        {
            entity.HasKey(e => e.AttendId).HasName("PK__Register__FDE0E8249D12A3E6");

            entity.ToTable("RegisterAttend");

            entity.Property(e => e.AttendId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AttendName).HasMaxLength(100);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.WorkshopId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.RegisterAttends)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__RegisterA__Custo__160F4887");

            entity.HasOne(d => d.Workshop).WithMany(p => p.RegisterAttends)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK__RegisterA__Works__17036CC0");
        });

        modelBuilder.Entity<RegisterCourse>(entity =>
        {
            entity.HasKey(e => e.EnrollCourseId).HasName("PK__Register__2FE6979159C1992B");

            entity.ToTable("RegisterCourse");

            entity.Property(e => e.EnrollCourseId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CourseId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EnrollCertId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EnrollChapterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EnrollQuizId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Course).WithMany(p => p.RegisterCourses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__RegisterC__Cours__17F790F9");

            entity.HasOne(d => d.EnrollCert).WithMany(p => p.RegisterCourses)
                .HasForeignKey(d => d.EnrollCertId)
                .HasConstraintName("FK__RegisterC__Enrol__18EBB532");

            entity.HasOne(d => d.EnrollChapter).WithMany(p => p.RegisterCourses)
                .HasForeignKey(d => d.EnrollChapterId)
                .HasConstraintName("FK__RegisterC__Enrol__19DFD96B");

            entity.HasOne(d => d.EnrollQuiz).WithMany(p => p.RegisterCourses)
                .HasForeignKey(d => d.EnrollQuizId)
                .HasConstraintName("FK__RegisterC__Enrol__1AD3FDA4");
        });

        modelBuilder.Entity<Shape>(entity =>
        {
            entity.HasKey(e => e.ShapeId).HasName("PK__Shape__70FC8381B85F314F");

            entity.ToTable("Shape");

            entity.Property(e => e.ShapeId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Element).HasMaxLength(20);
            entity.Property(e => e.ShapeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BD70BBA7E");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DocNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionName).HasMaxLength(100);
            entity.Property(e => e.TransactionType).HasMaxLength(50);
        });

        modelBuilder.Entity<VarietyColor>(entity =>
        {
            entity.HasKey(e => new { e.KoiVarietyId, e.ColorId }).HasName("PK__VarietyC__5C078AC6DA953AA8");

            entity.ToTable("VarietyColor");

            entity.Property(e => e.KoiVarietyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("KoiVarietyID");
            entity.Property(e => e.ColorId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Color).WithMany(p => p.VarietyColors)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VarietyCo__Color__1BC821DD");

            entity.HasOne(d => d.KoiVariety).WithMany(p => p.VarietyColors)
                .HasForeignKey(d => d.KoiVarietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VarietyCo__KoiVa__1CBC4616");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__84D4F90EABFF2584");

            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Wallet__Customer__1EA48E88");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.WalletTransactionId).HasName("PK__WalletTr__7184AEEF0D548270");

            entity.ToTable("WalletTransaction");

            entity.Property(e => e.WalletTransactionId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.WalletId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK__WalletTra__Walle__1F98B2C1");
        });

        modelBuilder.Entity<WorkShop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("PK__WorkShop__7A008C0A6D40A38E");

            entity.ToTable("WorkShop");

            entity.Property(e => e.WorkshopId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EntryFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.MasterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.WorkshopName).HasMaxLength(100);

            entity.HasOne(d => d.Master).WithMany(p => p.WorkShops)
                .HasForeignKey(d => d.MasterId)
                .HasConstraintName("FK__WorkShop__Master__208CD6FA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
