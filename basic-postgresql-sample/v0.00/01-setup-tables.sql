
DROP TABLE IF EXISTS __yuniql_schema_version;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS accounts;
DROP TABLE IF EXISTS movements;
DROP TABLE IF EXISTS uploads;
DROP TABLE IF EXISTS sessions;
DROP TABLE IF EXISTS transfers;


-- public.accounts definition

-- Drop table

-- DROP TABLE public.accounts;

CREATE TABLE public.accounts (
	accountid int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	userid int4 NOT NULL,
	created_at timestamptz NULL DEFAULT now(),
	currency varchar(15) NOT NULL,
	balance numeric(10) NOT NULL,
	updatedat timestamptz NULL DEFAULT now(),
	CONSTRAINT accounts_pkey PRIMARY KEY (accountid)
);


-- public.accounts foreign keys

ALTER TABLE public.accounts ADD CONSTRAINT fk_user FOREIGN KEY (userid) REFERENCES public.users(userid);


-- public.movements definition

-- Drop table

-- DROP TABLE public.movements;

CREATE TABLE public.movements (
	movementid int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	accountid int4 NOT NULL,
	amount numeric(10) NOT NULL,
	currency varchar(3) NOT NULL,
	movimenttime timestamptz NOT NULL DEFAULT now(),
	userid int4 NULL,
	CONSTRAINT movements_pkey PRIMARY KEY (movementid)
);


-- public.movements foreign keys

ALTER TABLE public.movements ADD CONSTRAINT accountid_fk FOREIGN KEY (accountid) REFERENCES public.accounts(accountid);
ALTER TABLE public.movements ADD CONSTRAINT fk_userid_movement FOREIGN KEY (userid) REFERENCES public.users(userid);


-- public.sessions definition

-- Drop table

-- DROP TABLE public.sessions;

CREATE TABLE public.sessions (
	id uuid NOT NULL,
	userid int4 NOT NULL,
	active bool NOT NULL DEFAULT true,
	created_at timestamptz NOT NULL DEFAULT now(),
	refresk_token varchar NULL,
	refresk_token_expire_at timestamptz NULL,
	tokenaccess varchar NULL,
	tokenaccessexpireat timestamptz NULL
);


-- public.sessions foreign keys

ALTER TABLE public.sessions ADD CONSTRAINT sessions_fkey FOREIGN KEY (userid) REFERENCES public.users(userid);


-- public.transfers definition

-- Drop table

-- DROP TABLE public.transfers;

CREATE TABLE public.transfers (
	transferid int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	originaccountid int4 NOT NULL,
	destinationaccountid int4 NOT NULL,
	amount numeric(10) NOT NULL,
	currency varchar(15) NOT NULL,
	transferdate timestamptz NOT NULL DEFAULT now(),
	CONSTRAINT accountsaredistinct_ck CHECK ((destinationaccountid <> originaccountid)),
	CONSTRAINT amountisvalid_ck CHECK ((amount > (0)::numeric)),
	CONSTRAINT transfers_pkey PRIMARY KEY (transferid)
);


-- public.uploads definition

-- Drop table

-- DROP TABLE public.uploads;

CREATE TABLE public.uploads (
	id int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	filename varchar(40) NULL,
	storedfilename varchar(40) NULL,
	contenttype varchar(40) NULL,
	userid int4 NULL,
	CONSTRAINT uploads_pkey PRIMARY KEY (id)
);


-- public.uploads foreign keys

ALTER TABLE public.uploads ADD CONSTRAINT fk_userid_upload FOREIGN KEY (userid) REFERENCES public.users(userid);

-- public.users definition

-- Drop table

-- DROP TABLE public.users;

CREATE TABLE public.users (
	userid int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	created_at timestamptz NULL DEFAULT now(),
	email varchar(50) NOT NULL,
	username varchar(25) NOT NULL,
	userpassword varchar(50) NOT NULL,
	passwordsalt varchar(250) NULL,
	usertoken varchar(600) NULL,
	updatedat timestamptz NULL DEFAULT now(),
	fullname varchar(50) NULL,
	CONSTRAINT users_email_key UNIQUE (email),
	CONSTRAINT users_pkey PRIMARY KEY (userid),
	CONSTRAINT users_username_key UNIQUE (username)
);

-- Table Triggers

create trigger set_timestamp before
update
    on
    public.users for each row execute function trigger_set_timestamp();