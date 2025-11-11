import { api } from "@/shared/api/httpClient";

export interface ImportResult {
    totalParsed: number;
    totalSaved: number;
    durationMs: number;
    status: string;
}

const IMPORT_ENDPOINT = "/api/imagenet/import";

export const importImageNet = async (): Promise<ImportResult> => {
    const { data } = await api.get<ImportResult>(IMPORT_ENDPOINT);
    return data;
};
