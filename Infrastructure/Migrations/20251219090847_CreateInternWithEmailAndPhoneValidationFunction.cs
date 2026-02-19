using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateInternWithEmailAndPhoneValidationFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION CreateInternWithValidation(
            p_first_name TEXT, 
            p_last_name TEXT, 
            p_email TEXT,
            p_phone TEXT,
            p_dept_id INT,
            p_univ_id INT
        )
        RETURNS TABLE (NewId INT, ErrorMessage TEXT) AS $$
        BEGIN
            -- 1. E-posta Kontrolü (Görseldeki step 1)
            IF EXISTS (SELECT 1 FROM ""Interns"" WHERE ""Email"" = p_email) THEN
                RETURN QUERY SELECT 0, 'Aynı gmaille stajyer veya çalışan kayıt edemezsiniz.'::TEXT;
                RETURN;
            END IF;

            -- 2. Telefon Kontrolü (Görseldeki step 2)
            IF EXISTS (SELECT 1 FROM ""Interns"" WHERE ""Phone"" = p_phone) THEN
                RETURN QUERY SELECT 0, 'Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz.'::TEXT;
                RETURN;
            END IF;

            -- 3. Kayıt İşlemi (Görseldeki step 3)
            INSERT INTO ""Interns"" (""FirstName"", ""LastName"", ""Email"", ""Phone"", ""DepartmentId"", ""UniversityId"", ""CreatedAt"")
            VALUES (p_first_name, p_last_name, p_email, p_phone, p_dept_id, p_univ_id, NOW())
            RETURNING ""Id"" INTO NewId;

            RETURN QUERY SELECT NewId, NULL::TEXT;
        END;
        $$ LANGUAGE plpgsql;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS CreateInternWithValidation(TEXT, TEXT, TEXT, TEXT, INT, INT);");
        }
    }
}
