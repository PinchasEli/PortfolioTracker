# Database Migration Guide - Manual Workflow

## 🚀 Quick Start (4 Commands)

```bash
# 1. Create migration
dotnet ef migrations add YourFeatureName

# 2. Generate SQL
dotnet ef migrations script --output temp.sql

# 3. Apply to Docker
docker exec -i postgres_db psql -U postgres -d portfolio_tracker_db < temp.sql

# 4. Clean up
rm temp.sql
```

---

## 📍 Always run from this directory:

```bash
cd C:\Users\Nachmani\Desktop\projects\PortfolioTracker\PortfolioTracker.API
```

Or in Git Bash:

```bash
cd ~/Desktop/projects/PortfolioTracker/PortfolioTracker.API
```

---

## ✅ Complete Migration Workflow (Detailed)

### Step 1: Create a New Migration

```bash
# Make sure you're in PortfolioTracker.API directory
cd PortfolioTracker.API

# Create migration (give it a descriptive name)
dotnet ef migrations add YourMigrationName
```

**Example names:**

- `AddStockHoldings`
- `AddTransactionTable`
- `UpdatePortfolioFields`

---

### Step 2: Generate SQL Script

**⚠️ IMPORTANT:** Due to a Docker/EF Core issue, always use the manual SQL method:

```bash
# Generate SQL for ALL pending migrations
dotnet ef migrations script --output temp_migration.sql
```

**OR** for specific migration (safer - recommended):

```bash
# From last migration to your new one
dotnet ef migrations script [LastMigrationName] [NewMigrationName] --output temp_migration.sql
```

**Example:**

```bash
dotnet ef migrations script 20251225115552_AddPortfolioAndCashBalances 20251226120000_AddStockHoldings --output temp_migration.sql
```

---

### Step 3: Apply to Docker Database

```bash
# Apply the SQL to Docker PostgreSQL
docker exec -i postgres_db psql -U postgres -d portfolio_tracker_db < temp_migration.sql
```

You should see output like:

```
CREATE TABLE
CREATE INDEX
INSERT 0 1
COMMIT
```

---

### Step 4: Verify Migration Applied

```bash
# Check all tables
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "\dt"

# Check specific table structure
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "\d \"YourTableName\""

# Verify migration was recorded
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "SELECT * FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\";"
```

---

### Step 5: Clean Up

```bash
# Delete the temporary SQL file
rm temp_migration.sql
```

---

## 🔧 Troubleshooting

### If `dotnet ef database update` says "Done" but nothing changed:

**This is the known Docker issue.** Always use the manual SQL script method above instead.

### If you need to remove a migration (before applying):

```bash
dotnet ef migrations remove
```

### If migration was partially applied:

```bash
# Check what was actually applied
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "\dt"

# Manually delete the migration record from history
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "DELETE FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" = 'YourMigrationId';"

# Then reapply the corrected migration
```

### If you get "column cannot be cast automatically" error:

You need to add custom SQL in the migration's `Up()` method. Example:

```csharp
migrationBuilder.Sql(@"
    ALTER TABLE ""TableName""
    ALTER COLUMN ""ColumnName"" TYPE newtype
    USING ""ColumnName""::newtype;
");
```

---

## 📋 Your Database Connection

- **Host:** localhost
- **Port:** 5432
- **Database:** portfolio_tracker_db
- **Username:** postgres
- **Password:** 1234
- **Container:** postgres_db

---

## 🎯 Current Schema (as of 2025-12-25)

### Tables:

1. **Users** - Authentication & roles (Role as enum/integer)
2. **Customers** - Customer profiles linked to Users
3. **Portfolios** - Investment portfolios per customer
4. **PortfolioCashBalances** - Cash holdings per portfolio per currency

### Migrations Applied:

1. `20251225110644_InitialCreate` - Users, Customers
2. `20251225115552_AddPortfolioAndCashBalances` - Portfolios, CashBalances, Role enum conversion

### Key Relationships:

- User ←→ Customer (one-to-one)
- Customer → Portfolios (one-to-many)
- Portfolio → PortfolioCashBalances (one-to-many)

### Unique Constraints:

- `Portfolios`: (CustomerId, Name, Exchange) - No duplicate portfolio names per customer per exchange
- `PortfolioCashBalances`: (PortfolioId, Currency) - One balance per currency per portfolio

---

## 💡 Best Practices

1. **Always build first:** `dotnet build` to catch compilation errors
2. **Always verify:** Check Docker database after applying migrations
3. **Name descriptively:** Use clear migration names like `AddStockHoldingsTable`
4. **One feature per migration:** Don't combine unrelated database changes
5. **Test locally first:** Ensure your entities compile before creating migrations
6. **Keep migrations small:** Easier to debug and rollback if needed
7. **Review generated SQL:** Check `temp_migration.sql` before applying
8. **Never edit applied migrations:** Create a new migration to fix issues

---

## 📝 Quick Command Reference

```bash
# 1. Create migration
dotnet ef migrations add YourFeatureName

# 2. Generate SQL
dotnet ef migrations script --output temp.sql

# 3. Apply to Docker
docker exec -i postgres_db psql -U postgres -d portfolio_tracker_db < temp.sql

# 4. Clean up
rm temp.sql

# Optional: Verify tables
docker exec postgres_db psql -U postgres -d portfolio_tracker_db -c "\dt"
```
