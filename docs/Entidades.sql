CREATE TABLE "users" (
  "id" uuid PRIMARY KEY,
  "name" varchar2(255),
  "email" varchar2(255) UNIQUE,
  "created_at" timestamp
);

CREATE TABLE "clients" (
  "id" uuid PRIMARY KEY,
  "user_id" uuid,
  "name" varchar2(255),
  "email" varchar2(255),
  "phone" varchar2(255),
  "company_name" varchar2(255),
  "notes" text,
  "created_at" timestamp
);

CREATE TABLE "projects" (
  "id" uuid PRIMARY KEY,
  "user_id" uuid,
  "client_id" uuid,
  "title" varchar2(255),
  "description" text,
  "status" varchar2(255),
  "due_date" date,
  "created_at" timestamp
);

CREATE TABLE "tasks" (
  "id" uuid PRIMARY KEY,
  "project_id" uuid,
  "title" varchar2(255),
  "description" text,
  "status" varchar2(255),
  "due_date" date,
  "created_at" timestamp
);

CREATE TABLE "invoices" (
  "id" uuid PRIMARY KEY,
  "user_id" uuid,
  "client_id" uuid,
  "project_id" uuid,
  "issue_date" date,
  "due_date" date,
  "amount" decimal,
  "status" varchar2(255),
  "pdf_url" varchar2(255),
  "created_at" timestamp
);

CREATE TABLE "payments" (
  "id" uuid PRIMARY KEY,
  "invoice_id" uuid,
  "amount" decimal,
  "payment_date" date,
  "payment_method" varchar2(255),
  "notes" text,
  "created_at" timestamp
);

CREATE TABLE "expenses" (
  "id" uuid PRIMARY KEY,
  "user_id" uuid,
  "title" varchar2(255),
  "amount" decimal,
  "category" varchar2(255),
  "payment_date" date,
  "notes" text,
  "created_at" timestamp
);

ALTER TABLE "clients" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");

ALTER TABLE "projects" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");

ALTER TABLE "projects" ADD FOREIGN KEY ("client_id") REFERENCES "clients" ("id");

ALTER TABLE "tasks" ADD FOREIGN KEY ("project_id") REFERENCES "projects" ("id");

ALTER TABLE "invoices" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");

ALTER TABLE "invoices" ADD FOREIGN KEY ("client_id") REFERENCES "clients" ("id");

ALTER TABLE "invoices" ADD FOREIGN KEY ("project_id") REFERENCES "projects" ("id");

ALTER TABLE "payments" ADD FOREIGN KEY ("invoice_id") REFERENCES "invoices" ("id");

ALTER TABLE "expenses" ADD FOREIGN KEY ("user_id") REFERENCES "users" ("id");
