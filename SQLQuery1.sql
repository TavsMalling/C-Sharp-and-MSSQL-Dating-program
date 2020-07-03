--INSERT INTO [Preferences] VALUES ('Tavs', 21, 'Female', 'Female', 180, 'WhiteAAABrownAAABlack', 'BlueAAAGreenAAAGrey', 'SleepingAAAFuckingAAASuckingAAARiding')

SELECT Name, Age, Sex, Height, SkinColor, EyeColor, Interest FROM [Profile] WHERE Sex = 'Male' AND Age >= 15 AND Age <= 21 AND Height >= 180 AND Height <= 200
--SELECT Password FROM [Security] WHERE Username = 'TavsMalling'