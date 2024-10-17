import type { Contactverzoek } from "@/features/contact/contactverzoek/overzicht/types";

//api types

export type DigitaalAdresApiViewModel = {
  adres: string;
  soortDigitaalAdres?: string;
  omschrijving?: string;
};

export type ExpandedKlantContactApiViewmodel = {
  uuid: string;
  url: string;
  plaatsgevondenOp: string;
  kanaal: string;
  inhoud: string;
  onderwerp: string;
  hadBetrokkenActoren: Array<{
    soortActor: string;
    naam: string;
    actorIdentificator: {
      objectId: string;
    };
  }>;
  internetaak: InternetaakApiViewModel;
};

export type InternetaakApiViewModel = {
  uuid: string;
  url: string;
  nummer: string;
  gevraagdeHandeling: string;
  toegewezenAanActoren: Array<{
    uuid: string;
    url: string;
  }>;
  toelichting: string;
  status: string;
  toegewezenOp: string;
  afgehandeldOp: string;
  actor: ActorApiViewModel;
};

export type ActorApiViewModel = {
  uuid: string;
  url: string;
  naam: string;
  soortActor: string;
  indicatieActief: boolean;
};

//applicatie types

export type BetrokkeneMetKlantContact = {
  uuid: string;
  wasPartij: { uuid: string; url: string };
  klantContact: ExpandedKlantContactApiViewmodel;

  digitaleAdressen: Array<{ uuid: string; url: string }>;
  digitaleAdressenExpanded: Array<DigitaalAdresApiViewModel>;
  contactnaam: {
    achternaam: string;
    voorletters: string;
    voornaam: string;
    voorvoegselAchternaam: string;
  };
};

export interface MedewerkerIdentificatie {
  identificatie: string;
  achternaam: string;
  voorletters: string;
  voorvoegselAchternaam: string;
}

export interface ContactmomentViewModel {
  url: string;
  registratiedatum: string;
  kanaal: string;
  tekst: string;
  objectcontactmomenten: {
    object: string;
    objectType: "zaak";
    contactmoment: string;
  }[];
  medewerkerIdentificatie: MedewerkerIdentificatie;
}

export interface InternetaakPostModel {
  nummer: string;
  gevraagdeHandeling: string;
  aanleidinggevendKlantcontact: {
    uuid: string;
  };
  toegewezenAanActoren: {
    uuid: string;
  }[];
  toelichting: string;
  status: "te_verwerken" | "verwerkt";
  afgehandeldOp?: string;
}

export type SaveInterneTaakResponseModel = {
  data?: { url: string; gespreksId?: string };
  errorMessage?: string;
};

export type SaveKlantContactResponseModel = {
  data?: { url: string; uuid: string };
  errorMessage?: string;
};

export interface KlantContactPostmodel {
  uuid?: string;
  nummer?: string;
  kanaal: string;
  onderwerp: string;
  inhoud: string;
  indicatieContactGelukt: boolean;
  taal: string;
  vertrouwelijk: boolean;
  plaatsgevondenOp: string; // 2019-08-24T14:15:22Z
}




export type KlantBedrijfIdentifier =
  | {
      bsn: string;
    }
  | {
      vestigingsnummer: string;
    }
  | {
      rsin: string;
    }
  | {
      kvkNummer: string;
    }
  | {
      rsin: string;
      kvkNummer?: string;
    };

///////////////////////////////

//todo: Contactverzoek type verplaatsen naar hier. o meer specifieke models introduceren
export type ContactverzoekViewmodel = Contactverzoek;
