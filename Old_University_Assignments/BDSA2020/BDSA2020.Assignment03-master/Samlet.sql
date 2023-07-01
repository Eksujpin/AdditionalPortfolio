drop table if exists "User";
create table "User" (
	Id INT primary key,
	Name VARCHAR(100) not null,
	Email VARCHAR(100) not null UNIQUE
);
insert into "User" (Id, Name, Email) values (1, 'Tania Poxson', 'tpoxson0@gnu.org');
insert into "User" (Id, Name, Email) values (2, 'Janos Larraway', 'jlarraway1@huffingtonpost.com');
insert into "User" (Id, Name, Email) values (3, 'Ingram Byre', 'ibyre2@dyndns.org');


drop table if exists Tag;
create table Tag (
	Id INT primary key,
	Name VARCHAR(50) not null unique
);
insert into Tag (Id, Name) values (1, 'Easy');
insert into Tag (Id, Name) values (2, 'Medium');
insert into Tag (Id, Name) values (3, 'Hard');
insert into Tag (Id, Name) values (4, 'Programming');
insert into Tag (Id, Name) values (5, 'Admin work');
insert into Tag (Id, Name) values (6, 'Setup');
insert into Tag (Id, Name) values (7, 'Cleaning');


drop table if exists Task;
create table Task (
	Id INT primary key,
	Title VARCHAR(100) not null,
    AssignedTo int foreign key REFERENCES "User"(id) null,
	Description VARCHAR(max) null,
	State VARCHAR(8) not null
);
insert into Task (Id, Title, Description, assignedTo, State) values (1, 'Clean up code', 'deploy global platforms', 1, 'New');
insert into Task (Id, Title, Description, State) values (2, 'Program good stuff', 'deploy best-of-breed niches', 'New');
insert into Task (Id, Title, Description, State) values (3, 'Make citations', 'disintermediate 24/365 niches', 'Active');
insert into Task (Id, Title, Description, State) values (4, 'Cook dinner', 'productize integrated infomediaries', 'Active');
insert into Task (Id, Title, Description, State) values (5, 'Save the world', 'monetize customized experiences', 'Resolved');
insert into Task (Id, Title, Description, State) values (6, 'Destroy the world', 'incubate killer web-readiness', 'Resolved');
insert into Task (Id, Title, Description, assignedTo, State) values (7, 'Become famous', 'deploy visionary e-business', 2, 'Closed');
insert into Task (Id, Title, Description, State) values (8, 'Delete the sun', 'synergize granular platforms', 'Closed');
insert into Task (Id, Title, Description, assignedTo, State) values (9, 'Assassinate trump', 'enable 24/7 architectures', 2, 'Removed');
insert into Task (Id, Title, Description, assignedTo, State) values (10, 'Make america great again', 'whiteboard magnetic markets', 3, 'Removed');

drop table if exists TaskTag;
create table TaskTag (
	Id INT primary key,
	task int foreign key REFERENCES Task(id) not null, 
    tag int foreign key REFERENCES Tag(id) not null
);
insert into TaskTag (Id, task, tag) values (1, 1, 3);
insert into TaskTag (Id, task, tag) values (2, 1, 4);
insert into TaskTag (Id, task, tag) values (3, 1, 7);
insert into TaskTag (Id, task, tag) values (4, 2, 1);
insert into TaskTag (Id, task, tag) values (5, 2, 4);
insert into TaskTag (Id, task, tag) values (6, 5, 1);
insert into TaskTag (Id, task, tag) values (7, 9, 3);
insert into TaskTag (Id, task, tag) values (8, 9, 7);
insert into TaskTag (Id, task, tag) values (9, 9, 5);
insert into TaskTag (Id, task, tag) values (10, 10, 1);