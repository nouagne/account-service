CREATE TABLE IF NOT EXISTS accounts (
  account_id    uuid PRIMARY KEY,
  email         citext UNIQUE NOT NULL,
  password_hash text NOT NULL,
  first_name    text,
  last_name     text,
  locale        text DEFAULT 'fr-FR',
  time_zone     text DEFAULT 'Europe/Paris',
  status        text NOT NULL DEFAULT 'Active',
  created_at    timestamptz NOT NULL DEFAULT now(),
  updated_at    timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS refresh_tokens (
  token_id    uuid PRIMARY KEY,
  account_id  uuid NOT NULL REFERENCES accounts(account_id) ON DELETE CASCADE,
  token_hash  text NOT NULL,
  expires_at  timestamptz NOT NULL,
  created_at  timestamptz NOT NULL DEFAULT now(),
  UNIQUE(account_id, token_hash)
);
