import { ServiceResult } from "@/services";
import type { User } from "@/stores/user";
import { logoutUrl, meUrl } from "./config";

const anonymousUser = Object.freeze({
  isLoggedIn: false,
});

async function fetchUser(url: string): Promise<User> {
  const response = await fetch(url, {
    credentials: "include",
  });

  if (response.status === 401) return anonymousUser;

  if (!response.ok) {
    throw new Error("unexpected status code: " + response.status);
  }

  const json = await response.json();

  const roles = json?.roles;
  const id = json?.id;
  const email = json?.email;
  if (typeof id !== "string" || !id || typeof email !== "string" || !email)
    return anonymousUser;

  return {
    isLoggedIn: true,
    roles: Array.isArray(roles) ? roles : [],
    id,
    email,
  };
}

export const useCurrentUser = () => ServiceResult.fromFetcher(meUrl, fetchUser);

export const logOut = () =>
  fetch(logoutUrl, { credentials: "include" }).then((r) => {
    //TODO: what do we do if this fails?
    if (!r.ok) throw new Error("Login failed");
  });
