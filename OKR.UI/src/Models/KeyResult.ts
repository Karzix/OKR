import type { Sidequest } from "./Sidequests";

export class KeyResult {
  id: string | undefined;
  description: string | undefined;
  active: boolean | undefined;
  deadline: Date | undefined;
  unit: string | undefined;
  currentPoint: number | undefined;
  maximumPoint: number | undefined;
  objectId: string | undefined;
  sidequests: Sidequest[] | undefined;
}
