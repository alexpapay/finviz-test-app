import { api } from "@/shared/api/httpClient";
import type { SearchResultItem } from "../model/types";

export interface SearchResponse {
    items: SearchResultItem[];
    total: number;
}

const SEARCH_ENDPOINT = "/api/imagenet/search";

export const searchNodes = async (
    query: string,
    page: number,
    pageSize: number
): Promise<SearchResponse> => {
    if (!query.trim()) return { items: [], total: 0 };

    const skip = (page - 1) * pageSize;

    try {
        const { data } = await api.get<SearchResponse>(SEARCH_ENDPOINT, {
            params: { q: query, skip, take: pageSize },
        });
        return data;
    } catch (error) {
        return { items: [], total: 0 };
    }
};
