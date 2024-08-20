import {
  enforceOneOrZero,
  fetchLoggedIn,
  parseJson,
  parsePagination,
  throwIfNotOk,
} from "@/services";
import type { Klant } from "./types";

export const klantinteractiesBaseUrl = "/api/klantinteracties/api/v1";

export type Contactnaam = {
  voornaam: string;
  voorvoegselAchternaam?: string;
  achternaam: string;
};

export enum DigitaalAdresTypes {
  email = "email",
  telefoonnummer = "telnr",
}

type IdentificatorType = {
  codeRegister: string;
  codeSoortObjectId: string;
  codeObjecttype: string;
};

// TODO in toekomstige story: waardes overleggen met Maykin en INFO
export const identificatorTypes = {
  persoon: {
    codeRegister: "brp",
    codeSoortObjectId: "bsn",
    codeObjecttype: "inp",
  },
  vestiging: {
    codeRegister: "hr",
    codeSoortObjectId: "vtn",
    codeObjecttype: "vst",
  },
  nietNatuurlijkPersoonRsin: {
    codeRegister: "hr",
    codeSoortObjectId: "rsin",
    codeObjecttype: "innp",
  },
  nietNatuurlijkPersoonKvkNummer: {
    codeRegister: "hr",
    codeSoortObjectId: "kvkNummer",
    codeObjecttype: "innp",
  },
} satisfies Record<string, IdentificatorType>;

export enum PartijTypes {
  persoon = "persoon",
  organisatie = "organisatie",
  contactpersoon = "contactpersoon",
}

type Partij = {
  nummer?: string;
  uuid: string;
  url: string;
  partijIdentificatoren: { uuid: string }[];
  _expand?: {
    digitaleAdressen?: { adres?: string; soortDigitaalAdres?: string }[];
  };
};

export const fetchKlantById = (uuid: string) =>
  fetchLoggedIn(
    `${klantinteractiesBaseUrl}/partijen/${uuid}?${new URLSearchParams({ expand: "digitaleAdressen" })}`,
  )
    .then(throwIfNotOk)
    .then(parseJson)
    .then(mapPartijToKlant);

export function findKlantByIdentifier(
  query:
    | {
        vestigingsnummer: string;
      }
    | {
        rsin: string;
        kvkNummer?: string;
      }
    | {
        bsn: string;
      },
): Promise<Klant | null> {
  const expand = "digitaleAdressen";
  let soortPartij,
    partijIdentificator__codeSoortObjectId,
    partijIdentificator__objectId;

  if ("bsn" in query) {
    soortPartij = PartijTypes.persoon;
    partijIdentificator__codeSoortObjectId =
      identificatorTypes.persoon.codeSoortObjectId;
    partijIdentificator__objectId = query.bsn;
  } else {
    soortPartij = PartijTypes.organisatie;

    if ("vestigingsnummer" in query) {
      partijIdentificator__codeSoortObjectId =
        identificatorTypes.vestiging.codeSoortObjectId;
      partijIdentificator__objectId = query.vestigingsnummer;
    } else {
      partijIdentificator__codeSoortObjectId =
        identificatorTypes.nietNatuurlijkPersoonRsin.codeSoortObjectId;
      partijIdentificator__objectId = query.rsin;
    }
  }

  const searchParams = new URLSearchParams({
    expand,
    soortPartij,
    partijIdentificator__codeSoortObjectId,
    partijIdentificator__objectId,
  });

  return fetchLoggedIn(`${klantinteractiesBaseUrl}/partijen?${searchParams}`)
    .then(throwIfNotOk)
    .then(parseJson)
    .then((r) => parsePagination(r, (x) => mapPartijToKlant(x as Partij)))
    .then(enforceOneOrZero);
}

export async function createKlant(
  parameters:
    | {
        bsn: string;
        contactnaam: Contactnaam;
      }
    | {
        vestigingsnummer: string;
        naam: string;
      }
    | {
        rsin: string;
        kvkNummer?: string;
        naam: string;
      },
) {
  let partijIdentificatie, partijIdentificator, soortPartij;

  if ("bsn" in parameters) {
    soortPartij = PartijTypes.persoon;
    partijIdentificatie = { contactnaam: parameters.contactnaam };
    partijIdentificator = {
      ...identificatorTypes.persoon,
      objectId: parameters.bsn,
    };
  } else {
    soortPartij = PartijTypes.organisatie;
    partijIdentificatie = { naam: parameters.naam };
    partijIdentificator =
      "vestigingsnummer" in parameters
        ? {
            ...identificatorTypes.vestiging,
            objectId: parameters.vestigingsnummer,
          }
        : {
            ...identificatorTypes.nietNatuurlijkPersoonRsin,
            objectId: parameters.rsin,
          };
  }
  const partij = await createPartij(partijIdentificatie, soortPartij);

  const identificators = [
    await createPartijIdentificator({
      identificeerdePartij: {
        url: partij.url,
        uuid: partij.uuid,
      },
      partijIdentificator,
    }),
  ];

  if ("kvkNummer" in parameters && parameters.kvkNummer) {
    const kvkIdentificator = await createPartijIdentificator({
      identificeerdePartij: {
        url: partij.url,
        uuid: partij.uuid,
      },
      partijIdentificator: {
        ...identificatorTypes.nietNatuurlijkPersoonKvkNummer,
        objectId: parameters.kvkNummer,
      },
    });
    identificators.push(kvkIdentificator);
  }

  return mapPartijToKlant(partij, identificators);
}

const createPartijIdentificator = (body: {
  identificeerdePartij: {
    url: string;
    uuid: string;
  };
  partijIdentificator: IdentificatorType & {
    objectId: string;
  };
}) =>
  fetchLoggedIn(klantinteractiesBaseUrl + "/partij-identificatoren", {
    body: JSON.stringify(body),
    method: "POST",
    headers: {
      "content-type": "application/json",
    },
  })
    .then(throwIfNotOk)
    .then(parseJson);

const getPartijIdentificator = (uuid: string) =>
  fetchLoggedIn(klantinteractiesBaseUrl + "/partij-identificatoren/" + uuid)
    .then(throwIfNotOk)
    .then(parseJson);

function createPartij(
  partijIdentificatie: { naam: string } | { contactnaam: Contactnaam },
  soortPartij: PartijTypes,
) {
  return fetchLoggedIn(klantinteractiesBaseUrl + "/partijen", {
    body: JSON.stringify({
      digitaleAdressen: [],
      betrokkenen: [],
      categorieRelaties: [],
      voorkeursDigitaalAdres: null,
      vertegenwoordigden: [],
      rekeningnummers: [],
      partijIdentificatoren: [],
      voorkeursRekeningnummer: null,
      indicatieGeheimhouding: false,
      indicatieActief: true,
      partijIdentificatie,
      soortPartij,
    }),
    method: "POST",
    headers: {
      "content-type": "application/json",
    },
  })
    .then(throwIfNotOk)
    .then(parseJson);
}

async function mapPartijToKlant(
  partij: Partij,
  identificatoren?: any[],
): Promise<Klant> {
  if (!identificatoren?.length) {
    const promises = partij.partijIdentificatoren.map(({ uuid }) =>
      getPartijIdentificator(uuid),
    );
    identificatoren = await Promise.all(promises);
  }

  const getDigitaalAdres = (type: string) =>
    partij._expand?.digitaleAdressen?.find(
      (x) => x.adres && x.soortDigitaalAdres === type,
    )?.adres;

  const getIdentificator = (type: { codeSoortObjectId: string }) =>
    identificatoren?.find(
      (x) =>
        x?.partijIdentificator?.objectId &&
        x?.partijIdentificator?.codeSoortObjectId == type.codeSoortObjectId,
    )?.partijIdentificator?.objectId;

  return {
    _typeOfKlant: "klant",
    klantnummer: partij.nummer || "",
    id: partij.uuid,
    url: partij.url,
    telefoonnummer: getDigitaalAdres(DigitaalAdresTypes.telefoonnummer),
    emailadres: getDigitaalAdres(DigitaalAdresTypes.email),
    bsn: getIdentificator(identificatorTypes.persoon),
    vestigingsnummer: getIdentificator(identificatorTypes.vestiging),
    kvkNummer: getIdentificator(
      identificatorTypes.nietNatuurlijkPersoonKvkNummer,
    ),
    rsin: getIdentificator(identificatorTypes.nietNatuurlijkPersoonRsin),
  };
}

export function searchKlantenByDigitaalAdres(
  query:
    | {
        telefoonnummer: string;
        partijType: PartijTypes;
      }
    | {
        email: string;
        partijType: PartijTypes;
      },
) {
  let key, value;

  if ("telefoonnummer" in query) {
    key = DigitaalAdresTypes.telefoonnummer;
    value = query.telefoonnummer;
  } else {
    key = DigitaalAdresTypes.email;
    value = query.email;
  }

  const searchParams = new URLSearchParams();
  searchParams.append("verstrektDoorPartij__soortPartij", query.partijType);
  searchParams.append("soortDigitaalAdres", key);
  searchParams.append("adres__icontains", value);

  const url = klantinteractiesBaseUrl + "/digitaleadressen?" + searchParams;

  return (
    fetchLoggedIn(url)
      .then(throwIfNotOk)
      .then(parseJson)
      .then(
        ({
          results,
        }: {
          results: { verstrektDoorPartij: { uuid: string } }[];
        }) => {
          const partijIds = results.map((x) => x.verstrektDoorPartij.uuid);
          const uniquePartijIds = [...new Set(partijIds)];
          const promises = uniquePartijIds.map(fetchKlantById);
          return Promise.all(promises);
        },
      )
      // TIJDELIJK: de filters werken nog niet in OpenKlant 2.1, dat komt in een nieuwe release
      // daarom filteren we hier handmatig
      .then((klanten) =>
        klanten.filter((klant) => {
          const isBedrijf =
            !!klant.kvkNummer || !!klant.vestigingsnummer || !!klant.rsin;
          if (!isBedrijf) return false;
          const matchesEmail =
            key === DigitaalAdresTypes.email &&
            klant.emailadres?.includes(value);
          const matchesTelefoon =
            key === DigitaalAdresTypes.telefoonnummer &&
            klant.telefoonnummer?.includes(DigitaalAdresTypes.telefoonnummer);
          return !!matchesEmail || !!matchesTelefoon;
        }),
      )
  );
}
