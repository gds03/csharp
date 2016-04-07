create table [Log]
(
	[id] int primary key identity(1, 1),
	
	[date] datetime,
	[thread] nvarchar(255),
	[level] nvarchar(50),
	[logger] nvarchar(255),
	[message] nvarchar(max),
	[exception] nvarchar(2000)
);




