/*--------------------------------------------------------------
-- Use the next lines if you want to drop/create database

use [master]
go

drop database [FlowUsers]
go

create database [FlowUsers]
go

use [FlowUsers]
go
*/

IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'AddressDetails'))
BEGIN
	RAISERROR ('The database is not empty, please drop the database before running this script', 15, 1)
END


/*--------------------------------------------------------------

CREATE DATABASE

--------------------------------------------------------------*/

CREATE TABLE [dbo].[AddressDetails] (
    [AddressDetailsId] [int] NOT NULL IDENTITY,
    [Address] [nvarchar](200),
    [City] [nvarchar](50),
    [Region] [nvarchar](50),
    [PostalCode] [nvarchar](20),
    [Country] [nvarchar](50),
    CONSTRAINT [PK_dbo.AddressDetails] PRIMARY KEY ([AddressDetailsId])
)
CREATE TABLE [dbo].[Domain] (
    [DomainId] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [Phone] [nvarchar](20),
    [Fax] [nvarchar](20),
    [Email] [nvarchar](256),
    [AddressDetailsId] [int],
    [DateCreated] [datetime] NOT NULL,
    [DateLastUpdated] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.Domain] PRIMARY KEY ([DomainId])
)
CREATE TABLE [dbo].[DomainUser] (
    [DomainUserId] [int] NOT NULL IDENTITY,
    [UserId] [int] NOT NULL,
    [DomainId] [int] NOT NULL,
    CONSTRAINT [PK_dbo.DomainUser] PRIMARY KEY ([DomainUserId])
)
CREATE TABLE [dbo].[User] (
    [UserId] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](16) NOT NULL,
    [Email] [nvarchar](256) NOT NULL,
    [Password] [nvarchar](100) NOT NULL,
    [IsActive] [bit] NOT NULL,
    [FirstName] [nvarchar](50),
    [LastName] [nvarchar](50),
    [Title] [nvarchar](5),
    [Gender] [nvarchar](5),
    [Birthday] [datetime],
    [Photo] [varbinary](max),
    [PhotoPath] [nvarchar](256),
    [Note] [nvarchar](600),
    [HomePhone] [nvarchar](20),
    [MobilePhone] [nvarchar](20),
    [WorkPhone] [nvarchar](20),
    [Position] [nvarchar](50),
    [Department] [nvarchar](50),
    [HireDate] [datetime],
    [OnlineStatusId] [int],
    [AddressDetailsId] [int],
    [DateCreated] [datetime] NOT NULL,
    [DateLastUpdated] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.User] PRIMARY KEY ([UserId])
)
CREATE TABLE [dbo].[OnlineStatus] (
    [OnlineStatusId] [int] NOT NULL IDENTITY,
    [Code] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](200),
    CONSTRAINT [PK_dbo.OnlineStatus] PRIMARY KEY ([OnlineStatusId])
)
CREATE TABLE [dbo].[Resource] (
    [ResourceId] [int] NOT NULL IDENTITY,
    [Type] [nvarchar](20),
    [Display] [nvarchar](20),
    [Value] [nvarchar](100),
    [Order] [int] NOT NULL,
    [Description] [nvarchar](200),
    [RoleId] [int],
    CONSTRAINT [PK_dbo.Resource] PRIMARY KEY ([ResourceId])
)
CREATE TABLE [dbo].[Role] (
    [RoleId] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](20) NOT NULL,
    [Description] [nvarchar](200),
    [DateCreated] [datetime] NOT NULL,
    [DateLastUpdated] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.Role] PRIMARY KEY ([RoleId])
)
CREATE TABLE [dbo].[RoleUser] (
    [RoleUserId] [int] NOT NULL IDENTITY,
    [IsPrimary] [bit] NOT NULL,
    [UserId] [int] NOT NULL,
    [RoleId] [int] NOT NULL,
    CONSTRAINT [PK_dbo.RoleUser] PRIMARY KEY ([RoleUserId])
)
CREATE TABLE [dbo].[UserFollowing] (
    [UserFollowingId] [int] NOT NULL IDENTITY,
    [FollowerUserId] [int] NOT NULL,
    [FollowingUserId] [int] NOT NULL,
    CONSTRAINT [PK_dbo.UserFollowing] PRIMARY KEY ([UserFollowingId])
)
CREATE INDEX [IX_AddressDetailsId] ON [dbo].[Domain]([AddressDetailsId])
CREATE INDEX [IX_UserId] ON [dbo].[DomainUser]([UserId])
CREATE INDEX [IX_DomainId] ON [dbo].[DomainUser]([DomainId])
CREATE INDEX [IX_OnlineStatusId] ON [dbo].[User]([OnlineStatusId])
CREATE INDEX [IX_AddressDetailsId] ON [dbo].[User]([AddressDetailsId])
CREATE INDEX [IX_RoleId] ON [dbo].[Resource]([RoleId])
CREATE INDEX [IX_UserId] ON [dbo].[RoleUser]([UserId])
CREATE INDEX [IX_RoleId] ON [dbo].[RoleUser]([RoleId])
CREATE INDEX [IX_FollowerUserId] ON [dbo].[UserFollowing]([FollowerUserId])
CREATE INDEX [IX_FollowingUserId] ON [dbo].[UserFollowing]([FollowingUserId])
ALTER TABLE [dbo].[Domain] ADD CONSTRAINT [FK_dbo.Domain_dbo.AddressDetails_AddressDetailsId] FOREIGN KEY ([AddressDetailsId]) REFERENCES [dbo].[AddressDetails] ([AddressDetailsId])
ALTER TABLE [dbo].[DomainUser] ADD CONSTRAINT [FK_dbo.DomainUser_dbo.Domain_DomainId] FOREIGN KEY ([DomainId]) REFERENCES [dbo].[Domain] ([DomainId])
ALTER TABLE [dbo].[DomainUser] ADD CONSTRAINT [FK_dbo.DomainUser_dbo.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
ALTER TABLE [dbo].[User] ADD CONSTRAINT [FK_dbo.User_dbo.AddressDetails_AddressDetailsId] FOREIGN KEY ([AddressDetailsId]) REFERENCES [dbo].[AddressDetails] ([AddressDetailsId])
ALTER TABLE [dbo].[User] ADD CONSTRAINT [FK_dbo.User_dbo.OnlineStatus_OnlineStatusId] FOREIGN KEY ([OnlineStatusId]) REFERENCES [dbo].[OnlineStatus] ([OnlineStatusId])
ALTER TABLE [dbo].[Resource] ADD CONSTRAINT [FK_dbo.Resource_dbo.Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
ALTER TABLE [dbo].[RoleUser] ADD CONSTRAINT [FK_dbo.RoleUser_dbo.Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId]) ON DELETE CASCADE
ALTER TABLE [dbo].[RoleUser] ADD CONSTRAINT [FK_dbo.RoleUser_dbo.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId]) ON DELETE CASCADE
ALTER TABLE [dbo].[UserFollowing] ADD CONSTRAINT [FK_dbo.UserFollowing_dbo.User_FollowerUserId] FOREIGN KEY ([FollowerUserId]) REFERENCES [dbo].[User] ([UserId])
ALTER TABLE [dbo].[UserFollowing] ADD CONSTRAINT [FK_dbo.UserFollowing_dbo.User_FollowingUserId] FOREIGN KEY ([FollowingUserId]) REFERENCES [dbo].[User] ([UserId])
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] [nvarchar](150) NOT NULL,
    [ContextKey] [nvarchar](300) NOT NULL,
    [Model] [varbinary](max) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201408211230032_AutomaticMigration', N'Flow.Users.Data.Migrations.Configuration',  0x1F8B0800000000000400ED5DDD6EDCB815BE2FD07710E6AA2DB21E3B8B045B63BC8B641C778D8D63C3E36C7B67D023DA1656234D254ED646D127EB451FA9AF5052BFFC174951D238081608D614F9F1F0F03BFC39E41CFEEF3FFF5DFCF4B489832F30CBA33439991D1D1CCE0298ACD3304A1E4E663B74FFDD0FB39F7EFCE31F161FC2CD53F06B9DEF7B920F974CF293D92342DBE3F93C5F3FC20DC80F36D13A4BF3F41E1DACD3CD1C84E9FCF5E1E15FE7474773882166182B0816D7BB04451B58FC81FF5CA6C91A6ED10EC4176908E3BC4AC75F56056AF0096C60BE056B78323B8BD3DF0F3EE75892835380C02C781747008BB182F1FD2C0049922280B090C738CF0A6569F2B0DAE20410DF3C6F21CE770FE21C56C21FB7D94DDB71F89AB463DE16ACA1D6BB1CA51B4BC0A3EF2BC5CCF9E24EEA9D358AC3AAFB80558C9E49AB0BF59DCCDE856106F3FC142210C5F92CE0AB3C5EC619C92EE8F8609966F0802DFE2AE032BD6AD8814944FE7B152C7731DA65F024813B9481F85570B5BB8BA3F52FF0F926FD0D2627C92E8E6991B1D0F81B938093AEB2740B33F47C0DEFA50D390F67C19C4599F3300D8812A16CF87982BE7F3D0B3E61C1C05D0C1BB6504A5A21AC8CBFC1046600C1F00A200433DCD9E7212CF42DC822AFB9AE1033145BDA2CB8004F1F61F2801E4F66B83F67C159F404C33AA512E2731261C3C48550B6835DF52C0B599495BCF151C7357C28983A6C2D57698E40BCC4F9B54AF3A1B3140F4B9967B52DE6AD216ACDF314DB6294589B65596C4A732C257031C3B6E458E647FEEDDFBF82801D0C7E4C93C1C97B069E86AEE203EEAD5857C99BB71E6AE9189BF58531F5E13283841B75399274136DECBB8D14FC0872F4791BBAE17D025FA28782B6DA26CE826B1817D9F2C7685BAE642AB3BEE5739E65E9E63A8D1BDBE132DCAED25DB6265C4B75B96E40F60051AF718A8C348E6315499D7EBC2255BB8F5975E9B1C62DB3FA3AF86C38DA9AB2B89EB094EC2542DFD6B978E6521F15ACA573C818AB93ADA4A75EB2328F54AEE21F8D54C53FBDACC8C97EA6B61C579B19DB5A3A66F923B369CAD2BC7CCD8EB68B0B90E7BFA759A86BAFE18EC2B2E6F3FCDD1A455F1A4DBF4FB19980C41AE72CCA72E46761A6AF884CE663D47313A1585B89873AB07184640419B492F751861E43F02C2E7D3A17BC286D681125806CAA28D92EC85A55269D012C1E0C1E075F837E4A91AE03DF7AD9A2FF9C6EE0285B838BF42E8AC7A9EAEF69F6DB28155DA5798446F03E9CC22DC8D006CF3B43D7F47394416260B6C67699C451025778E6DCD9EE96BE6DB5C892B058CCA9375A92CFC2D25096C776C94AF7A35A4E36172725FD512E2393A3D7029615C472214B179E7241CBDB8EEDC2566B7B032E703BBCA1C3B8B14E61BECEA26DC7A8EBE2BE3626DD35CC2B565B12AE2E3825D96A195C8846971D8B64A49EA1A7F1D328DFC640E772F752CDAF20DE6977845E16749719B52477F4D10C6663124EE169A16BCE57CE56A4B07496AA997A5BE6686728E683303BB15F7BCD4C65C5B603042E34E9E0507586F5C020EBC4E95C2B86D6BAC756B13FCB5A2BC23BF913EB825313DFD5AF48971DCB00CEF3AB2CDA80F698D8D5EDE5C3A56F66FDFDC7F44ACFE2984E7F10C774E6AB37177E03CB3BF0990F6A69FC38EFCFD218DB4C31063978F19BD253BBF31B415CFDFA0CC0584658560A331F36D434C0094C495F5A44E576BEA9FB96CDCEEEEBE5B9A41B7C45565BDB6374622C7D935F2F7E95CD48FE3AAF8DB9BECBF3741D1502332795BC8F87D5C587240C8C4ED6CBE5477B9688D720D83EA32DB6482CCCC9EC2F829ABBA0EBC651D0BC3B8AADE2F0E0E048A805DB35CC88F990CB52498E478A2841E2201025EB680B621381B8C24E77F148373595F25F4EE1961C6324C844F3BEA4692AE506BB2EFD2DE614B14CF8461F7FEB09213D0BE779569A962DD76487E89D14F64A2E8904265DA9BA55E64028897AFB4A300289646E69555F6B7DD46D6FDB5248E7D69E60B0D28833FA50A5D1F80B1AA8C423052D15E4E70B3DF9253D92A040D9838641B92513C5A437F5C708F6BC92E9D98F1CE34E7E051F0CE6277639E871E263D68E1D44F53FE9D1B59B749FDC0DE23AE1D14AED57FB08A461BDC6AA9E55B890DB7E6DCF87CCE922F73CD3A085D363A881475ABD497FC93DC6566C91EAB35FDD637085F14629BB55EE9A62BBD57668917BB43AB872C4AB7471999CC2182218900B74E4A7624B90AF41287A17F00E37F4C13199D863714CD60F2F8763DA294CE170F4C431BBC96B6F3836C1AC27ED87BD9FF3340E40DD82B9CB1BC812857253DBADCB3B3C89E32CA6BA05198961DD8A371184775BEF0DED6ACFAD0D210437EE40C4E35DC0D3318F936442EA71BA37E71E75CC3102F94AFF3C2E83700998D59260398AF3AEE2730465D707F1F7EA102DAF8EED79CA10E41544AC3F8560B587020AF794C03E16AADC42C9806A67A91140D1422548C9DC0E2015845161DA1720550BEB57E940AB770A32A076D3D50582F92C052816CD0685550A699759061A6D0C41A55A6AE4E2E0289AF3DD2D5C20A6F26A7FD2C95BA1E93954D3348AB182499B9E3C51609425F10B45B6F9C6AAA1CF41946A511E96181E9708EAA8B8D2A912D90149B76E1D9421F3E74BD4D1E9F63776FC53ADE85686CED53F103B44E7B34A1D6A0FB5A18FDA5E1552AF3405C30FAF3E6DA51CCAB49622AED68C7CABFDAC84598675A9D44105DCD55151011A2FA1819F9092989ACE344D977B0669987242EBDF70F67E95A4E16A979781D38B93B8BBCFE56EAE211BAEA2BCC60F63E089716FF8D054D7ECFD1583A089A7C0D657C0358C5E19758C8F1DDE819134D65C3832539974976BBDCFF5A7347E67DB436BF55DA866AFD57C5BCCCB186A55C262AE08B6B6B800DB2D69465BB24A095665E4B5E5772BFBA8649B1263BE66266E7E67D8D484D20C3C40EE6BF97BA7E267E2E486E41D20F7F096E146C826EE2C15DB81BA3E7EF328F65FBD3FA84B90FF6FB7B1F4DD4EFD9AAD55E7196E21F93D69D15828AEAD940001098707629075FE8A7399C6BB4D627AD3A013590A6883530623A34196924B9E3A843AD4188D51A799A3D0A1C468243ADDA25575B830A66175A288B398730C101C3002DD044F3DCB5F2376D73B983EDC966FCC0C38AD2AA8D2697BE78A56AAFA26961AA9FCA10A8D52A658B0A5FC1D3B439432C91CA388CA45231409E6E5ABA022344295646DC5038C0FCC6F65980EA33FD8E131BF97E131998F7B6661E594DDDFCA644B14634B9317D65B5BED10162D4EE52A5623CAB0EC51EC4781897ABE7F9F3BF6B65D3FFBE995FE23AA8FD1AC8D77C40CCC4DAA39521BBF88466A532D46F936821133D6B7C9E6586D90221AAA4D3547AAC210D130559239461D668806A9D3CC51DA3842344E9B6A352793C842DC9C4C922C31CA3042024E996C611345A020C6268A1473042A0A100D43259B6331817E6834E683391E15CD8746A392ADD6DE55C01E6EE55DA55ACC0E54481E667EA0D22DF4DF84DD61D4DFA49A23F1B76F69BCAE9BB96AD46FABB7F6FB207338E7C9EF3399EBCE290C26757DF17169276E926DB7C7CC8FF6593BA53EEC0D0DDA23813E0C68CEC2ED7B5F5D54ED146983C2B08E1155A0191D5A19EE85592D1429163D5E0773617ABB4E34C7A9A2B5D0285592C5609C09EB962A6978062BFBABBAD1CAF495E296EB7476501CEDF4B201C9199509FFA5C5FCE872B82D8D6F8E7C9B949BAEECBFB96EEE05B9B1D16E935D9790B1D27EB34D85196177A74DF2D84E9917317A7127727D3D33ED553037178DA6BCAEB398981A7CAF692276E870F9BBC78CA3A2E35E72172A75AB5484D55C391D9826C2A1289FA5A9BD391CE50E4117D58164F7B354C2096599651660857D8942723AB97ACE11DC94E45AFD335EC651B155AD335C8024BA87392A63BA9068563F708F5BEDCF4353F33C0F63C9812E152347FBCBF5899F798A88DE3B43D058C691E15E764ABE806CFD08323144598B6BFB909314F38D3524FB6E932750F199268506AC15C0BECAE424AEED234C0E6CF5F70AD220EC2CD7B5C62AB47FD7C8536F53CF18794264DE6590639298F176A0DA9145F835C37912C2A793D9BF8AC2C7C1F93F6EF9F2AF8262777A1C1C06FFB6954512B1902CE791C7888586782E6F08399B9A7C51E1F680CF202627A9C280196529151F9C1EFA31ADBA2E6751B9DD7B370E7DEDDACB23F46FE7907AF4D61AD37AB0727A23462EEDA1FD0CC03F017317A1FECFBF785A90F0AFBD7882651E779163DA42B26FB9F8C1E49F6E918DE046CB3AFA29172CD95DF19ACB9F36E0E9CF4E50F4F32DBE2663FAB51629E65BFB65B7F0388BA7D588E42D164FC8C2D32B9E70F997563CD991F8B08A2760FE1D1557E6CBDF76309D4BD9D27D9677DF969ABD161E9AE866133E1132C882A473F3EDB0C793845E77F36CD8BFF1E1D0597E9FD818A493CA4355AF2334F788862754E6CD0C9315A3F51319857EA7E2A3840069CC77BEC1F05A96321F54ED9EB170B101E9998CED4B12D3EC985E4F38407D15B39DFCD8728A9718062190F0F882CB867332BFCC10034C5F9F8CE28472DA070406E18EFCCD00D35E604BF7A281E2C1013B499AE21E88D137603E1FD442137BC7208A261F7AC029E4BE53CCA7CE5F70DA9F5106F6C15B5F52FC68DB40F74EB1E765B1289CC304F788E93B45D8FAAF2D50FD504385C08901D930F53061CC8B3D19243A42F850BDC8060352C62ADB5B6674DCFFB7752E0DC38A171524DEA8F7F671CA30EE0ED5DDB8E1A60B758DFB13E83D6DCFD8EC62B08BD19B1C03C43B07411E3B78FB0B0AA6DDBFCFBDC6B916E33E3906EFDE7FAEA82FC2EF2B577C4C0B5F0D57C69C4DACB832E95C621A16CDE77A82F599F5087ABDF73CE8FAFD8358E98B0E6FFE8D1583B2621F028F8B51E4F8FE9406156F3C12ADF754E1B2287FF371320BEFC895A9D20FEB147D5C137C5C56877560F28EB8E4EA3ACAEF1DF5286A50631BA1B23E00019DFD2CABC529E0B936DEB9AC16AB68E8CA60E852E4D4224CBA364ABA0ADDB877A9114DDACDD477557FEF4B9475B991CB02328A0397E8DEA64A2B8C726FA2A81B08ABF4A40825A5CCD99318E97E7A5868B8CF26FB8C822E1B27C5F0225334D56B887323214727B0BF10E6D4ECD08647D0358F9FB1D8A0221E9A967A0C526ED9346E52637FA1EFB1692A5A9A8621B727E5D04DD3EC917D061AB76BB4740921F9D1FC30CD1F246AF8D40AB008002EFEA61DEFB47609B98656FE750AF3E8A1855860CC04AE993D5693E73CB94FEB3D1F27519D85FF05049E6C43BC017B97A1E81EAC11FEBCC67370D1DCE2CE29F915D41D0CCF93CB1DDAEE106E32DCDCC54CC40BB265D4D55F443967655E5C167704731F4DC06246E4E6DE65F27E17C56123F799E4CE8D0282EC45ABCB4DA42F11B9E4F4F0DC207D12A2EBA9802AF5355BE81BB8D9C6182CBF4C5680FC32CB5E36CCC18FF001AC9FEBD0046A90EE8E60D5BE388DC04306367985D196C77F620E879BA71FFF0F6700267E97AC0000 , N'6.1.1-30610')


exec sp_executesql N'INSERT [dbo].[AddressDetails]([Address], [City], [Region], [PostalCode], [Country])
VALUES (@0, @1, @2, @3, @4)
SELECT [AddressDetailsId]
FROM [dbo].[AddressDetails]
WHERE @@ROWCOUNT > 0 AND [AddressDetailsId] = scope_identity()',N'@0 nvarchar(200),@1 nvarchar(50),@2 nvarchar(50),@3 nvarchar(20),@4 nvarchar(50)',@0=N'11 aa',@1=N'Macondo',@2=N'America',@3=N'21456',@4=N'America'
go
exec sp_executesql N'INSERT [dbo].[Domain]([Name], [Phone], [Fax], [Email], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, NULL, NULL, NULL, NULL, @1, @2)
SELECT [DomainId]
FROM [dbo].[Domain]
WHERE @@ROWCOUNT > 0 AND [DomainId] = scope_identity()',N'@0 nvarchar(50),@1 datetime2(7),@2 datetime2(7)',@0=N'AcmeStar',@1='2014-05-14 22:33:37.0726601',@2='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[OnlineStatus]([Code], [Description])
VALUES (@0, @1)
SELECT [OnlineStatusId]
FROM [dbo].[OnlineStatus]
WHERE @@ROWCOUNT > 0 AND [OnlineStatusId] = scope_identity()',N'@0 nvarchar(50),@1 nvarchar(200)',@0=N'Online',@1=N'User is online'
go
exec sp_executesql N'INSERT [dbo].[OnlineStatus]([Code], [Description])
VALUES (@0, @1)
SELECT [OnlineStatusId]
FROM [dbo].[OnlineStatus]
WHERE @@ROWCOUNT > 0 AND [OnlineStatusId] = scope_identity()',N'@0 nvarchar(50),@1 nvarchar(200)',@0=N'Offline',@1=N'User is offline'
go
exec sp_executesql N'INSERT [dbo].[OnlineStatus]([Code], [Description])
VALUES (@0, @1)
SELECT [OnlineStatusId]
FROM [dbo].[OnlineStatus]
WHERE @@ROWCOUNT > 0 AND [OnlineStatusId] = scope_identity()',N'@0 nvarchar(50),@1 nvarchar(200)',@0=N'Busy',@1=N'User is busy'
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, NULL)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int',@0=N'page',@1=N'Home',@2=N'#',@3=1
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, NULL)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int',@0=N'page',@1=N'Task',@2=N'#/tasks',@3=2
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, NULL)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int',@0=N'page',@1=N'Holidays',@2=N'#/holidays',@3=6
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'MgrDev',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'Admin',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, @4)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int,@4 int',@0=N'page',@1=N'Dashboard',@2=N'#/dashboard',@3=3,@4=2
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'Approver',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'Dev',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'MgrBA',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, @4)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int,@4 int',@0=N'page',@1=N'Sketch',@2=N'#/sketch',@3=4,@4=5
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'BA',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, @4)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int,@4 int',@0=N'page',@1=N'Sketch',@2=N'#/sketch',@3=4,@4=6
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'Finance',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'MgrFinance',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'PM',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go

exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'MgrPM',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, @4)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int,@4 int',@0=N'page',@1=N'Report',@2=N'#/report',@3=5,@4=9
go
exec sp_executesql N'INSERT [dbo].[Resource]([Type], [Display], [Value], [Order], [Description], [RoleId])
VALUES (@0, @1, @2, @3, NULL, @4)
SELECT [ResourceId]
FROM [dbo].[Resource]
WHERE @@ROWCOUNT > 0 AND [ResourceId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 int,@4 int',@0=N'page',@1=N'Report',@2=N'#/report',@3=5,@4=10
go

exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'HR',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[Role]([Name], [Description], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3)
SELECT [RoleId]
FROM [dbo].[Role]
WHERE @@ROWCOUNT > 0 AND [RoleId] = scope_identity()',N'@0 nvarchar(20),@1 nvarchar(200),@2 datetime2(7),@3 datetime2(7)',@0=N'MgrHR',@1=N'Role description',@2='2014-05-14 22:33:37.0726601',@3='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'cgrant',@1=N'cgrant@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Cary',@5=N'Grant',@6=N'Images\users\cgrant.jpg',@7=N'Developer Manager',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=1,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=1,@2=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=0,@1=1,@2=2
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=0,@1=1,@2=3
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'pnewman',@1=N'pnewman@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Paul',@5=N'Newman',@6=N'Images\users\pnewman.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=2,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=2,@2=4
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=0,@1=2,@2=2
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'hbogart',@1=N'hbogart@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Humphrey',@5=N'Bogart',@6=N'Images\users\hbogart.jpg',@7=N'Business Analyst Manager',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=3,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=3,@2=5
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'knovak',@1=N'knovak@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Kim',@5=N'Novak',@6=N'Images\users\knovak.jpg',@7=N'Business Analyst',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=4,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=4,@2=6
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'mmonroe',@1=N'mmonroe@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Marilyn',@5=N'Monroe',@6=N'Images\users\mmonroe.jpg',@7=N'Accountant',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=5,@1=1
go

exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=5,@2=7
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'rredford',@1=N'rredford@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Robert',@5=N'Redford',@6=N'Images\users\rredford.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=6,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=6,@2=4
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=0,@1=6,@2=2
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'apacino',@1=N'apacino@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Al',@5=N'Pacino',@6=N'Images\users\apacino.jpg',@7=N'Finance Manager',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=7,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=7,@2=8
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'cbronson',@1=N'cbronson@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Charles',@5=N'Bronson',@6=N'Images\users\cbronson.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=8,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=8,@2=4
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'dday',@1=N'dday@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Doris',@5=N'Day',@6=N'Images\users\dday.jpg',@7=N'Accountant',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=9,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=9,@2=7
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'gkelly',@1=N'gkelly@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Grace',@5=N'Kelly',@6=N'Images\users\gkelly.jpg',@7=N'Business Analyst',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=10,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=10,@2=6
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'gpeck',@1=N'gpeck@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Gregory',@5=N'Peck',@6=N'Images\users\gpeck.jpg',@7=N'Project Manager',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=11,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=11,@2=9
go

exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'jdean',@1=N'jdean@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'James',@5=N'Dean',@6=N'Images\users\jdean.jpg',@7=N'Project Manager',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=12,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=12,@2=9
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'jstewart',@1=N'jstewart@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'James',@5=N'Stewart',@6=N'Images\users\jstewart.jpg',@7=N'PM Manager',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=13,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=13,@2=10
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'mpfeiffer',@1=N'mpfeiffer@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Michelle',@5=N'Pfeiffer',@6=N'Images\users\mpfeiffer.jpg',@7=N'Business Analyst',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=14,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=14,@2=6
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'nkidman',@1=N'nkidman@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Nicole',@5=N'Kidman',@6=N'Images\users\nkidman.jpg',@7=N'Human Resources',@8=N'HR',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=15,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=15,@2=11
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'rdeniro',@1=N'rdeniro@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Robert',@5=N'Deniro',@6=N'Images\users\rdeniro.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=16,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=16,@2=4
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=0,@1=16,@2=3
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'rhayworth',@1=N'rhayworth@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Rita',@5=N'Hayworth',@6=N'Images\users\rhayworth.jpg',@7=N'Project Manager',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=17,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=17,@2=9
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'rmitchum',@1=N'rmitchum@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Robert',@5=N'Mitchum',@6=N'Images\users\rmitchum.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go

exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=18,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=18,@2=4
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'sdee',@1=N'sdee@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Sandra',@5=N'Dee',@6=N'Images\users\sdee.jpg',@7=N'Accountant',@8=N'Finance',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=19,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=19,@2=7
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'smcqueen',@1=N'smcqueen@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Steve',@5=N'Mcqueen',@6=N'Images\users\smcqueen.jpg',@7=N'Developer',@8=N'IT',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=20,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=20,@2=4
go
exec sp_executesql N'INSERT [dbo].[User]([Name], [Email], [Password], [IsActive], [FirstName], [LastName], [Title], [Gender], [Birthday], [Photo], [PhotoPath], [Note], [HomePhone], [MobilePhone], [WorkPhone], [Position], [Department], [HireDate], [OnlineStatusId], [AddressDetailsId], [DateCreated], [DateLastUpdated])
VALUES (@0, @1, @2, @3, @4, @5, NULL, NULL, NULL, NULL, @6, NULL, NULL, NULL, NULL, @7, @8, NULL, NULL, NULL, @9, @10)
SELECT [UserId]
FROM [dbo].[User]
WHERE @@ROWCOUNT > 0 AND [UserId] = scope_identity()',N'@0 nvarchar(16),@1 nvarchar(256),@2 nvarchar(16),@3 bit,@4 nvarchar(50),@5 nvarchar(50),@6 nvarchar(256),@7 nvarchar(50),@8 nvarchar(50),@9 datetime2(7),@10 datetime2(7)',@0=N'tcruise',@1=N'tcruise@aa.com',@2=N'I/Wn9e+RVG0=',@3=1,@4=N'Tom',@5=N'Cruise',@6=N'Images\users\tcruise.jpg',@7=N'Human Resources Manager',@8=N'HR',@9='2014-05-14 22:33:37.0726601',@10='2014-05-14 22:33:37.0726601'
go
exec sp_executesql N'INSERT [dbo].[DomainUser]([UserId], [DomainId])
VALUES (@0, @1)
SELECT [DomainUserId]
FROM [dbo].[DomainUser]
WHERE @@ROWCOUNT > 0 AND [DomainUserId] = scope_identity()',N'@0 int,@1 int',@0=21,@1=1
go
exec sp_executesql N'INSERT [dbo].[RoleUser]([IsPrimary], [UserId], [RoleId])
VALUES (@0, @1, @2)
SELECT [RoleUserId]
FROM [dbo].[RoleUser]
WHERE @@ROWCOUNT > 0 AND [RoleUserId] = scope_identity()',N'@0 bit,@1 int,@2 int',@0=1,@1=21,@2=12
go
exec sp_executesql N'INSERT [dbo].[UserFollowing]([FoolowerUserId], [FollowingUserId])
VALUES (@0, @1)
SELECT [UserFollowingId]
FROM [dbo].[UserFollowing]
WHERE @@ROWCOUNT > 0 AND [UserFollowingId] = scope_identity()',N'@0 int,@1 int',@0=1,@1=2
go
exec sp_executesql N'INSERT [dbo].[UserFollowing]([FoolowerUserId], [FollowingUserId])
VALUES (@0, @1)
SELECT [UserFollowingId]
FROM [dbo].[UserFollowing]
WHERE @@ROWCOUNT > 0 AND [UserFollowingId] = scope_identity()',N'@0 int,@1 int',@0=2,@1=1
go
exec sp_executesql N'INSERT [dbo].[UserFollowing]([FoolowerUserId], [FollowingUserId])
VALUES (@0, @1)
SELECT [UserFollowingId]
FROM [dbo].[UserFollowing]
WHERE @@ROWCOUNT > 0 AND [UserFollowingId] = scope_identity()',N'@0 int,@1 int',@0=1,@1=3
go
exec sp_executesql N'INSERT [dbo].[UserFollowing]([FoolowerUserId], [FollowingUserId])
VALUES (@0, @1)
SELECT [UserFollowingId]
FROM [dbo].[UserFollowing]
WHERE @@ROWCOUNT > 0 AND [UserFollowingId] = scope_identity()',N'@0 int,@1 int',@0=1,@1=4
go
exec sp_executesql N'INSERT [dbo].[UserFollowing]([FoolowerUserId], [FollowingUserId])
VALUES (@0, @1)
SELECT [UserFollowingId]
FROM [dbo].[UserFollowing]
WHERE @@ROWCOUNT > 0 AND [UserFollowingId] = scope_identity()',N'@0 int,@1 int',@0=4,@1=5
go
