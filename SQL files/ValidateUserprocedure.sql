USE [ICDC]
GO
/****** Object:  StoredProcedure [dbo].[Validate_User]    Script Date: 10/01/18 13:09:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Validate_User]
      @Username NVARCHAR(20),
      @Password NVARCHAR(20)
AS
BEGIN
      SET NOCOUNT ON;
      DECLARE @UserId NVARCHAR(20), @LastLoginDate DATETIME
     
      SELECT @UserId = Username, @LastLoginDate = LastLoginDate
      FROM [ICDC$Applicants Register] WHERE Username = @Username AND [Password] = @Password
     
      IF @UserId IS NOT NULL
      BEGIN
            IF NOT EXISTS(SELECT UserId FROM UserActivation WHERE UserId = @UserId)
            BEGIN
                  UPDATE [ICDC$Applicants Register]
                  SET LastLoginDate = GETDATE()
                  WHERE Username = @UserId
                  SELECT @UserId [Username] -- User Valid
            END
            ELSE
            BEGIN
                  SELECT 'deactivated' -- User not activated.
            END
      END
      ELSE
      BEGIN
            SELECT 'invalid' -- User invalid.
      END
END
