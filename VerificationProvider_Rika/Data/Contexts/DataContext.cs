

using Microsoft.EntityFrameworkCore;
using VerificationProvider_Rika.Data.Entities;

namespace VerificationProvider_Rika.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VerificationRequestEntity> VerificationRequests { get; set; } = null!;


}
