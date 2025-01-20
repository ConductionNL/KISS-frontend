import { flushPromises } from "@vue/test-utils";
import { fetchLoggedIn, handleLogin } from "@/services";

import {
  describe,
  expect,
  test,
  afterEach,
  vi,
  afterAll,
  beforeAll,
} from "vitest";

import { setupServer } from "msw/node";
import { http } from "msw";

const someUrl = "https://dummy.request.fgdsf";

describe("fetchLoggedIn", () => {
  const consoleLogMock = vi.spyOn(console, "log");
  const consoleWarnMock = vi.spyOn(console, "warn");

  afterEach(() => {
    consoleLogMock.mockReset();
  });

  let status = 401;

  const server = setupServer(
    http.get(someUrl, () => {
      return new Response("test", {
        headers: { "Content-Type": "application/json" },
        status,
      });
    }),
  );

  // Start server before tests
  beforeAll(() => {
    server.listen();
  });

  afterAll(() => {
    server.close();
    server.resetHandlers();
  });

  test("should resolve when a 200 response is returned", async () => {
    status = 200;
    await fetchLoggedIn(someUrl, {
      method: "GET",
    });
    expect(consoleWarnMock).toHaveBeenCalledTimes(0);
    expect(consoleLogMock).toHaveBeenCalledTimes(0);
  });

  test("should retry when a 401 response is returned", async () => {
    status = 401;
    const promise = fetchLoggedIn(someUrl);
    await flushPromises();

    expect(consoleWarnMock).toHaveBeenLastCalledWith(
      "session expired. waiting for user to log in...",
    );

    status = 200;

    handleLogin();

    await promise;

    expect(consoleLogMock).toHaveBeenLastCalledWith(
      "user is logged in again, resuming...",
    );
  });
});
