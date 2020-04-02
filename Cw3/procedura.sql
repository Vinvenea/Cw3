CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN

DECLARE @IdStudies INT = (Select IdStudy From Studies Where Name = @Studies)

	IF @IdStudies IS NULL
	BEGIN
	--RAISERROR
		Return;
	END
    DECLARE @IdEnroll INT = (Select IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudies AND Semester = @Semester)
	DECLARE @IdEnrollment INT = (Select IdEnrollment From Enrollment Where IdStudy = @IdStudies AND Semester = @Semester+1)
	IF @IdEnrollment IS NULL
	BEGIN
		DECLARE @value INTEGER
		SELECT @value = max(IdEnrollment) +1 from Enrollment
		Insert into Enrollment(IdEnrollment,Semester,IdStudy, StartDate) VALUES (@value, @Semester+1,@IdStudies,GETDATE())
		UPDATE Student SET IdEnrollment = @IdEnrollment WHERE IdEnrollment = @IdEnroll
	END
	ELSE
	BEGIN
		UPDATE Student SET IdEnrollment = @IdEnrollment WHERE IdEnrollment = @IdEnroll
	END

END;

exec PromoteStudents 'Inf', 1;